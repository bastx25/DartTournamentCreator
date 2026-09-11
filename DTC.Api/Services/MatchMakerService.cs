using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Enums;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using DTC.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Frozen;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Principal;

namespace DTC.Api.Services
{
    public class MatchMakerService : IMatchMakerService
    {
        private readonly DartDbContext _context;
        private readonly ITournamentPlayerRepository _tournamentPlayerRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly Random _random = Random.Shared;

        private readonly IRoundRepository _roundRepo;
        private readonly IBoardService _boardService;

        public MatchMakerService(DartDbContext context,
            ITournamentPlayerRepository tournamentPlayerRepository,
            IMatchRepository matchRepository,
            IGroupRepository groupRepository,
            IRoundRepository roundRepository,
            IBoardService boardService)
        {
            _context = context;
            _tournamentPlayerRepo = tournamentPlayerRepository;
            _matchRepo = matchRepository;
            _groupRepo = groupRepository;
            _roundRepo = roundRepository;
            _boardService = boardService;
        }

        public async Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options)
        {
            ArgumentNullException.ThrowIfNull(options);

            await _groupRepo.DeleteAllTournamentGroups(tournamentId);

            await _tournamentPlayerRepo.SetTournamentPlayers(tournamentId, options.PlayerIds);

            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPlayers)
                .Include(t => t.Config)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            #region Validation
            if (tournament == null)
                throw new KeyNotFoundException($"Turnier {tournamentId} wurde nicht gefunden.");

            ValidateGroupOptions(options);

            var playerIds = options.PlayerIds?.ToList();
            if (playerIds is not null)
            {
                if (playerIds.Count == 0)
                    throw new InvalidOperationException("Die Spielerliste darf nicht leer sein.");

                if (playerIds.Count != playerIds.Distinct().Count())
                    throw new InvalidOperationException("Ein Spieler darf in PlayerIds nicht mehrfach vorkommen.");
            }

            var tournamentPlayers = playerIds is null
                ? tournament.TournamentPlayers.ToList()
                : tournament.TournamentPlayers
                    .Where(tp => playerIds.Contains(tp.PlayerId))
                    .ToList();

            if (playerIds is not null && tournamentPlayers.Count != playerIds.Count)
            {
                var foundIds = tournamentPlayers.Select(tp => tp.PlayerId).ToHashSet();
                var missing = playerIds.Where(id => !foundIds.Contains(id)).ToArray();
                throw new KeyNotFoundException(
                    $"Folgende Spieler gehören nicht zum Turnier: {string.Join(", ", missing)}.");
            }

            if (tournamentPlayers.Count == 0)
                throw new InvalidOperationException("Für das Turnier wurden keine Spieler gefunden.");

            if (options.GroupCount > tournamentPlayers.Count)
                throw new InvalidOperationException(
                    "Die Anzahl der Gruppen darf die Anzahl der ausgewählten Spieler nicht überschreiten.");

            if (options.QualifiersPerGroup > tournamentPlayers.Count / options.GroupCount)
            {
                throw new InvalidOperationException(
                    "Die Anzahl der Qualifikanten pro Gruppe ist größer als die maximal mögliche Gruppengröße.");
            }

            var hasGeneratedData = await _context.Groups.AnyAsync(g => g.TournamentId == tournamentId)
                || await _context.Rounds.AnyAsync(r => r.TournamentId == tournamentId);
            if (hasGeneratedData)
                throw new InvalidOperationException("Für dieses Turnier wurden bereits Gruppen oder Runden generiert.");
            #endregion Validation

            // Shuffle first, then distribute round-robin. This gives groups whose sizes differ by at most one.
            var shuffledPlayers = tournamentPlayers.OrderBy(_ => _random.Next()).ToList();
            var groups = new List<Group>(options.GroupCount);
            

            //Generate Empty Groups
            for (var index = 0; index < options.GroupCount; index++)
            {
                groups.Add(new Group
                {
                    TournamentId = tournamentId,
                    Sequence = index + 1,
                    Name = CreateGroupName(index),
                    QualifiersCount = options.QualifiersPerGroup
                });
            }

            //distríbute shuffled players into groups e.g. 5 Player 2 groups : Group A: 3 Players; GroupB : 2 Players
            for (var index = 0; index < shuffledPlayers.Count; index++)
            {
                var group = groups[index % groups.Count];
                group.GroupPlayers.Add(new GroupPlayer
                {
                    TournamentPlayerId = shuffledPlayers[index].Id
                });
            }

            var startTime = options.StartTime ?? tournament.StartDate;
            var matchDuration = options.MatchDurationMinutes ?? tournament.Config.MatchDurationMinutes;
            var breakMinutes = options.BreakBetweenMatchesMinutes ?? tournament.Config.BreakBetweenMatchesMinutes;

            ValidateSchedule(startTime, matchDuration, breakMinutes);


            await ScheduleGroupMatches(groups);
            await _boardService.SetBoards(groups, startTime, matchDuration, breakMinutes);

            await _context.Groups.AddRangeAsync(groups);
            await _context.SaveChangesAsync();
        }

        private async Task ScheduleGroupMatches(List<Group> groups)
        {
            var scheduledMatches = new List<MatchCandidate>();

            foreach (var group in groups)
            {
                var players = group.GroupPlayers
                    .Select(gp => gp.TournamentPlayerId)
                    .ToList();

                var candidates = CreateRoundRobinCandidates(group, players);
                scheduledMatches.AddRange(candidates);
            }

            var orderedMatches = OrderMatchesFairly(scheduledMatches);

            foreach (var candidate in orderedMatches)
            {
                var match = new Match
                {
                    Group = candidate.Group,
                    Status = candidate.IsBye ? MatchStatus.Completed : MatchStatus.Scheduled,
                    PlannedStart = null,
                    PlannedEnd = null,
                    ActualEnd = null,
                    //TODO: Get available Board
                    BoardId = 1,
                };

                foreach (var tournamentPlayerId in candidate.PlayerIds)
                {
                    match.Participants.Add(new MatchParticipant
                    {
                        TournamentPlayerId = tournamentPlayerId,
                        Score = 0,
                        IsWinner = candidate.IsBye
                    });
                }

                await _context.Matches.AddAsync(match);
            }
        }


        public async Task GenerateKnockoutAsync(int tournamentId, GenerateGroupsDto options)
        {
            var qualifiedPlayers = await CheckAndHandleForTiebreaks(tournamentId, options);

            var requiredKnockoutPlayers =
                options.QualifiersPerGroup * options.GroupCount;

            if(qualifiedPlayers.Count() !=  requiredKnockoutPlayers)
            {
                throw new InvalidOperationException("Tiebreaks still running");
            }

            await CreateKnockoutGroups(tournamentId, options,qualifiedPlayers);
        }

        private async Task CreateKnockoutGroups(int tournamentId, GenerateGroupsDto options, List<TournamentPlayer> qualifiedPlayers)
        {
            var koGroup = await CreateTiebreakerGroup(tournamentId, GetKoGroupName(qualifiedPlayers.Count),
                qualifiedPlayers, options.StartTime ??  DateTimeOffset.Now, 
                options.MatchDurationMinutes ?? 20, 
                options.BreakBetweenMatchesMinutes ?? 5);

            await _context.AddAsync(koGroup);
            await _context.SaveChangesAsync();
        }

        private string GetKoGroupName(int count)
        {
            return count switch
            {
                2 => "Finale",
                4 => "Halbfinale",
                8 => "Viertelfinale",
                _ => $"{count / 2}. Finale"
            };
        }

        private async Task<List<TournamentPlayer>> CheckAndHandleForTiebreaks(int tournamentId, GenerateGroupsDto options)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPlayers)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            #region Validation

            if (tournament == null)
                throw new KeyNotFoundException($"Turnier {tournamentId} wurde nicht gefunden.");

            ValidateGroupOptions(options);

            #endregion

            await _roundRepo.DeleteAllTournamentRoundsAsync(tournamentId);

            var groups = await _groupRepo.GetGroupsAsync(tournamentId);

            var groupResults = await GetGroupResults(groups);

            // ---------------------------------------------------------
            // Spieler bestimmen, die direkt für das Knockout qualifiziert
            // sind bzw. in den Tiebreak kommen.
            // ---------------------------------------------------------

            List<TournamentPlayer> knockoutPlayers;
            List<TournamentPlayer> tiebreakPlayers;

            if (tournament.TournamentPlayers.Any(x => x.IsQualified))
            {
                // Bereits vorhandene Qualifikation verwenden
                knockoutPlayers = tournament.TournamentPlayers
                    .Where(x => x.IsQualified)
                    .ToList();

                tiebreakPlayers = new List<TournamentPlayer>();
            }
            else
            {
                (knockoutPlayers, tiebreakPlayers) =
                    GetKnockoutPlayers(
                        groupResults,
                        options.QualifiersPerGroup);
            }

            // ---------------------------------------------------------
            // Tiebreaker-Gruppe suchen
            // ---------------------------------------------------------

            var tiebreakerGroup = groups
                .LastOrDefault(g =>
                    g.Name.Contains(MatchStatus.Tiebreaker.ToString()));

            var tiebreakerGroups = tiebreakerGroup != null
                ? new List<Group> { tiebreakerGroup }
                : new List<Group>();

            // ---------------------------------------------------------
            // Anzahl der noch benötigten Qualifikanten berechnen
            // ---------------------------------------------------------

            var requiredKnockoutPlayers =
                options.QualifiersPerGroup * options.GroupCount;

            var qualifiersFromTiebreak =
                requiredKnockoutPlayers - knockoutPlayers.Count;

            // ---------------------------------------------------------
            // Tiebreaker-Ergebnisse laden
            // ---------------------------------------------------------

            var (qualifiers, tiebreakPlayersFromGroup) =
                await GetQualifiersFromTiebreaker(
                    tiebreakerGroups,
                    Math.Max(0, qualifiersFromTiebreak));

            // Falls noch keine Tiebreaker-Gruppe existiert,
            // die zuvor ermittelten Tiebreak-Spieler verwenden.
            if (!tiebreakerGroups.Any())
            {
                tiebreakPlayersFromGroup = tiebreakPlayers;
            }

            // ---------------------------------------------------------
            // Qualifizierte Spieler setzen
            // ---------------------------------------------------------

            var allQualifiedPlayers = knockoutPlayers
                .Concat(qualifiers)
                .Distinct()
                .ToList();

            foreach (var player in tournament.TournamentPlayers)
            {
                player.IsQualified = allQualifiedPlayers.Contains(player);
            }

            // ---------------------------------------------------------
            // Tiebreaker-Gruppe ggf. erstellen
            // ---------------------------------------------------------

            var startTime = options.StartTime ?? tournament.StartDate;
            var matchDuration =
                options.MatchDurationMinutes ??
                tournament.Config.MatchDurationMinutes;

            var breakMinutes =
                options.BreakBetweenMatchesMinutes ??
                tournament.Config.BreakBetweenMatchesMinutes;

            if (tiebreakPlayersFromGroup.Any())
            {
                var createdTiebreakerGroup =
                    await CreateTiebreakerGroup(
                        tournament.Id,
                        MatchStatus.Tiebreaker.ToString(),
                        tiebreakPlayersFromGroup,
                        startTime,
                        matchDuration,
                        breakMinutes);

                await _context.Groups.AddAsync(createdTiebreakerGroup);
            }

            await _context.SaveChangesAsync();

            return allQualifiedPlayers;
        }

        private async Task<(List<TournamentPlayer> Qualifiers, List<TournamentPlayer> TiebreakPlayers)>
        GetQualifiersFromTiebreaker(
        List<Group> tiebreakerGroups,
        int qualifiersFromTiebreak)
        {
            if (qualifiersFromTiebreak <= 0 || tiebreakerGroups.Count == 0)
            {
                return (
                    new List<TournamentPlayer>(),
                    new List<TournamentPlayer>());
            }

            var groupResults = await GetGroupResults(
                tiebreakerGroups,
                includeTiebreakers: true);

            return GetKnockoutPlayers(
                groupResults,
                qualifiersFromTiebreak);
        }



        private async Task<Group> CreateTiebreakerGroup(int tournamentId, string name ,List<TournamentPlayer> tiebreakPlayers, DateTimeOffset startTime, int matchDuration, int breakMinutes)
        {
            var groups = new List<Group> { new Group { 
                TournamentId = tournamentId,
                Sequence = _context.Groups.Where(x=> x.TournamentId == tournamentId).Max(x => x.Sequence) + 1,
                Name = name, 
                GroupPlayers = tiebreakPlayers.Select(x => new GroupPlayer { TournamentPlayerId = x.Id }).ToList(),
            }};
                
            await ScheduleGroupMatches(groups);

            await _boardService.SetBoards(groups, startTime, matchDuration, breakMinutes);

            return groups.First();
        }

        private async Task<List<GroupResult>> GetGroupResults(
            IEnumerable<Group> groups,
            bool includeTiebreakers = false)
        {
            var results = new List<GroupResult>();

            foreach (var group in groups)
            {
                if (!includeTiebreakers &&
                    group.Name.Contains(MatchStatus.Tiebreaker.ToString()))
                {
                    continue;
                }

                var matches = await _matchRepo.GetByGroupIdAsync(group.Id);

                var playerWins = matches
                    .SelectMany(match => match.Participants)
                    .Where(participant => participant.IsWinner)
                    .GroupBy(participant => participant.TournamentPlayer.Id)
                    .Select(g => new GroupPlayerResult(
                        g.First().TournamentPlayer,
                        g.Count()))
                    .OrderByDescending(x => x.Wins)
                    .ToList();

                results.Add(new GroupResult(
                    group.Id,
                    playerWins));
            }

            return results;
        }

        public sealed record GroupResult(
    int GroupId,
    List<GroupPlayerResult> Players);

        public sealed record GroupPlayerResult(
            TournamentPlayer Player,
            int Wins);




        private (List<TournamentPlayer> KnockoutPlayers, List<TournamentPlayer> TiebreakPlayers) GetKnockoutPlayers(
        IEnumerable<GroupResult> groups,
        int qualifiersPerGroup)
        {
            var knockoutPlayers = new List<TournamentPlayer>();
            var tiebreakPlayers = new List<TournamentPlayer>();

            foreach (var group in groups)
            {

                var players = group.Players
                    .OrderByDescending(x => x.Wins)
                    .ToList();

                if (players.Count == 0)
                    continue;

                // Alle Spieler bis zur Qualifikationsgrenze
                var qualifiedCandidates = players
                    .Take(qualifiersPerGroup)
                    .ToList();

                // Wenn weniger Spieler als Qualifikationsplätze vorhanden sind
                if (qualifiedCandidates.Count < qualifiersPerGroup)
                {
                    knockoutPlayers.AddRange(
                        qualifiedCandidates.Select(x => x.Player));

                    continue;
                }

                var cutoffWins = qualifiedCandidates.Last().Wins;

                // Spieler mit mehr Siegen als der Cutoff sind fix qualifiziert
                var fixedQualified = players
                    .Where(x => x.Wins > cutoffWins)
                    .ToList();

                knockoutPlayers.AddRange(
                    fixedQualified.Select(x => x.Player));

                // Spieler mit exakt den Cutoff-Siegen kämpfen um die
                // verbleibenden Qualifikationsplätze
                var tiedPlayers = players
                    .Where(x => x.Wins == cutoffWins)
                    .ToList();

                var remainingSlots =
                    qualifiersPerGroup - fixedQualified.Count;

                if (tiedPlayers.Count <= remainingSlots)
                {
                    knockoutPlayers.AddRange(
                        tiedPlayers.Select(x => x.Player));
                }
                else
                {
                    tiebreakPlayers.AddRange(
                        tiedPlayers.Select(x => x.Player));
                }
            }

            return (knockoutPlayers, tiebreakPlayers);
        }


        public sealed record GroupWinner(int GroupId, MatchParticipant Player);

        public Task AdvanceKnockoutAsync(int matchId)
        {
            throw new NotImplementedException();
        }

        private static void ValidateGroupOptions(GenerateGroupsDto options)
        {
            if (options.GroupCount is < 1 or > 64)
                throw new InvalidOperationException("Die Anzahl der Gruppen muss zwischen 1 und 64 liegen.");

            if (options.QualifiersPerGroup is < 1 or > 64)
                throw new InvalidOperationException("Die Anzahl der Qualifikanten pro Gruppe muss zwischen 1 und 64 liegen.");

            if (options.PlayersPerGroup is <= 0)
                throw new InvalidOperationException("PlayersPerGroup muss größer als 0 sein.");
        }

        private static void ValidateSchedule(DateTimeOffset startTime, int duration, int breakMinutes)
        {
            if (duration is < 1 or > 240)
                throw new InvalidOperationException("Die Matchdauer muss zwischen 1 und 240 Minuten liegen.");

            if (breakMinutes is < 0 or > 120)
                throw new InvalidOperationException("Die Pause muss zwischen 0 und 120 Minuten liegen.");

            if (startTime == default)
                throw new InvalidOperationException("StartTime ist ungültig.");
        }

        /// <summary>
        /// Creates an alphabetical group name based on a zero-based index.
        /// The naming follows a spreadsheet-like pattern: A, B, ..., Z, AA, AB, ...
        /// </summary>
        /// <param name="zeroBasedIndex">The zero-based index used to generate the group name.</param>
        /// <returns>An alphabetical group name corresponding to the specified index.</returns>
        private static string CreateGroupName(int zeroBasedIndex)
        {
            var value = zeroBasedIndex + 1;
            var result = string.Empty;

            while (value > 0)
            {
                value--;
                result = (char)('A' + (value % 26)) + result;
                value /= 26;
            }

            return "Gruppe " + result;
        }


        private static List<MatchCandidate> CreateRoundRobinCandidates(
    Group group,
    List<int> players)
        {
            var result = new List<MatchCandidate>();

            for (var i = 0; i < players.Count; i++)
            {
                for (var j = i + 1; j < players.Count; j++)
                {
                    result.Add(new MatchCandidate(
                        group,
                        new[] { players[i], players[j] },
                        false));
                }
            }

            return result;
        }

        private List<MatchCandidate> OrderMatchesFairly(List<MatchCandidate> candidates)
        {
            // Byes are completed administrative records and do not consume a scheduled
            // match slot. Keep them out of the fairness heuristic for playable matches.
            var playable = candidates.Where(c => !c.IsBye).ToList();
            var byes = candidates.Where(c => c.IsBye).OrderBy(_ => _random.Next()).ToList();

            var remaining = playable
                .OrderBy(_ => _random.Next())
                .ToList();
            var ordered = new List<MatchCandidate>(candidates.Count);
            var lastPosition = new Dictionary<int, int>();

            while (remaining.Count > 0)
            {
                var best = remaining
                    .Select(candidate => new
                    {
                        Candidate = candidate,
                        Consecutive = candidate.PlayerIds.Count(id =>
                            lastPosition.TryGetValue(id, out var position) && position == ordered.Count - 1),
                        MaxRecency = candidate.PlayerIds
                            .Select(id => lastPosition.TryGetValue(id, out var position)
                                ? ordered.Count - position
                                : int.MaxValue)
                            .DefaultIfEmpty(int.MaxValue)
                            .Min()
                    })
                    .OrderBy(x => x.Consecutive)
                    .ThenByDescending(x => x.MaxRecency)
                    .ThenBy(_ => _random.Next())
                    .First();

                ordered.Add(best.Candidate);
                remaining.Remove(best.Candidate);

                foreach (var playerId in best.Candidate.PlayerIds)
                    lastPosition[playerId] = ordered.Count - 1;
            }

            ordered.AddRange(byes);
            return ordered;
        }
     

        private sealed record MatchCandidate(Group Group, IReadOnlyList<int> PlayerIds, bool IsBye);

    }
}
