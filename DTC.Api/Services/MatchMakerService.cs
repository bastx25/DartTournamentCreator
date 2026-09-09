using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Enums;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using DTC.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

        public MatchMakerService(DartDbContext context,
            ITournamentPlayerRepository tournamentPlayerRepository,
            IMatchRepository matchRepository,
            IGroupRepository groupRepository,
            IRoundRepository roundRepository)
        {
            _context = context;
            _tournamentPlayerRepo = tournamentPlayerRepository;
            _matchRepo = matchRepository;
            _groupRepo = groupRepository;
            _roundRepo = roundRepository;
        }

        public async Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options)
        {
            ArgumentNullException.ThrowIfNull(options);

            await _groupRepo.DeleteAllTournamentGroups(tournamentId);

            await _tournamentPlayerRepo.SetTournamentPlayers(tournamentId, options.PlayerIds);

            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPlayers)
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
            var matchDuration = options.MatchDurationMinutes ?? tournament.MatchDurationMinutes;
            var breakMinutes = options.BreakBetweenMatchesMinutes ?? tournament.BreakBetweenMatchesMinutes;

            ValidateSchedule(startTime, matchDuration, breakMinutes);


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
            var currentStart = startTime;

            foreach (var candidate in orderedMatches)
            {
                var match = new Match
                {
                    Group = candidate.Group,
                    Status = candidate.IsBye ? MatchStatus.Completed : MatchStatus.Scheduled,
                    PlannedStart = candidate.IsBye ? null : currentStart,
                    PlannedEnd = candidate.IsBye ? null : currentStart.AddMinutes(matchDuration),
                    ActualEnd = candidate.IsBye ? currentStart : null
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

                if (!candidate.IsBye)
                    currentStart = currentStart.AddMinutes(matchDuration + breakMinutes);
            }

            //TODO: Update Tournament witch Options

            await _context.Groups.AddRangeAsync(groups);
            await _context.SaveChangesAsync();
        }

        public async Task GenerateKnockoutAsync(int tournamentId, GenerateGroupsDto options)
        {
            await _roundRepo.DeleteAllTournamentRoundsAsync(tournamentId);

            var groups = await _groupRepo.GetGroupsByTournamentIdAsync(tournamentId);

            var groupResults = await GetGroupResults(groups);

            var (knockoutPlayers, tiebreakPlayers) =
                GetKnockoutPlayers(groupResults, options.QualifiersPerGroup);

            if (tiebreakPlayers.Count > 0)
            {
                //var tiebreakerRound = await CreateTiebreakerRoundAsync(
                //    tournamentId,
                //    tiebreakPlayers);

                //await CreateTiebreakerMatchesAsync(
                //    tiebreakerRound,
                //    tiebreakPlayers);
            }

            // knockoutPlayers -> direkt in die KnockoutPlayerListe
            // tiebreakPlayers -> werden erst nach den Tiebreaker-Matches
            //                    in die KnockoutPlayerListe übernommen
        }

        private async Task<List<GroupResult>> GetGroupResults(IEnumerable<Group> groups)
        {
            var results = new List<GroupResult>();

            foreach (var group in groups)
            {
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




        private (
    List<TournamentPlayer> KnockoutPlayers,
    List<TournamentPlayer> TiebreakPlayers)
    GetKnockoutPlayers(
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

        //public async Task GenerateKnockoutAsync(int tournamentId)
        //{
        //    var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
        //    if (tournament == null)
        //        throw new KeyNotFoundException($"Turnier {tournamentId} wurde nicht gefunden.");

        //    var groups = await _context.Groups
        //        .Include(g => g.GroupPlayers)
        //            .ThenInclude(gp => gp.TournamentPlayer)
        //        .Where(g => g.TournamentId == tournamentId)
        //        .OrderBy(g => g.Sequence)
        //        .ToListAsync();

        //    if (groups.Count == 0)
        //        throw new InvalidOperationException("Vor der K.-o.-Phase muss die Gruppenphase generiert werden.");

        //    var groupStageRounds = await _context.Rounds
        //        .Where(r => r.TournamentId == tournamentId && r.Phase == RoundPhase.GroupStage)
        //        .OrderBy(r => r.Sequence)
        //        .ToListAsync();

        //    if (groupStageRounds.Count == 0)
        //        throw new InvalidOperationException("Es wurde keine Gruppenrunde gefunden.");

        //    var groupStageMatches = await _context.Matches
        //        .Where(m => m.Round.TournamentId == tournamentId && m.Round.Phase == RoundPhase.GroupStage)
        //        .Include(m => m.Participants)
        //        .ToListAsync();

        //    if (groupStageMatches.Any(m => m.Status != MatchStatus.Completed))
        //        throw new InvalidOperationException("Die Gruppenphase ist noch nicht vollständig abgeschlossen.");

        //    if (await _context.Rounds.AnyAsync(r => r.TournamentId == tournamentId && r.Phase == RoundPhase.Knockout))
        //        throw new InvalidOperationException("Für dieses Turnier wurde bereits eine K.-o.-Phase generiert.");

        //    var qualifiers = new List<QualifiedPlayer>();
        //    foreach (var group in groups)
        //    {
        //        if (group.QualifiersCount < 1)
        //            throw new InvalidOperationException($"Gruppe {group.Name} hat keine gültige Qualifikantenanzahl.");

        //        if (group.QualifiersCount > group.GroupPlayers.Count)
        //        {
        //            throw new InvalidOperationException(
        //                $"Gruppe {group.Name} hat nur {group.GroupPlayers.Count} Spieler, aber {group.QualifiersCount} Qualifikanten.");
        //        }

        //        var rankedPlayers = group.GroupPlayers
        //            .Select(gp => BuildStanding(gp.TournamentPlayerId, group.Id, groupStageMatches))
        //            .OrderByDescending(s => s.Wins)
        //            .ThenByDescending(s => s.ScoreDifference)
        //            .ThenByDescending(s => s.TotalScore)
        //            .ThenBy(s => s.TournamentPlayerId)
        //            .Take(group.QualifiersCount)
        //            .ToList();

        //        for (var rank = 0; rank < rankedPlayers.Count; rank++)
        //        {
        //            var standing = rankedPlayers[rank];
        //            qualifiers.Add(new QualifiedPlayer(
        //                standing.TournamentPlayerId,
        //                group.Id,
        //                group.Sequence,
        //                rank + 1));
        //        }
        //    }

        //    if (qualifiers.Count < 2)
        //        throw new InvalidOperationException("Für die K.-o.-Phase werden mindestens zwei qualifizierte Spieler benötigt.");

        //    var bracketSize = NextPowerOfTwo(qualifiers.Count);
        //    if (bracketSize > 4096)
        //        throw new InvalidOperationException("Die erzeugte K.-o.-Runde wäre zu groß.");

        //    var firstRoundSlots = BuildKnockoutSlots(qualifiers, bracketSize);
        //    var nextSequence = Math.Max(
        //        await GetNextRoundSequenceAsync(tournamentId),
        //        groupStageRounds.Max(r => r.Sequence) + 1);

        //    var duration = tournament.MatchDurationMinutes;
        //    var breakMinutes = tournament.BreakBetweenMatchesMinutes;
        //    var firstStart = groupStageRounds
        //        .Select(r => r.PlannedEnd)
        //        .Where(end => end.HasValue)
        //        .Select(end => end!.Value)
        //        .DefaultIfEmpty(tournament.StartDate)
        //        .Max();

        //    var rounds = new List<Round>();
        //    var roundMatchCount = bracketSize / 2;
        //    var roundStart = firstStart;

        //    for (var roundIndex = 0; roundIndex < Log2(bracketSize); roundIndex++)
        //    {
        //        var round = new Round
        //        {
        //            TournamentId = tournamentId,
        //            Sequence = nextSequence + roundIndex,
        //            Name = GetKnockoutRoundName(roundMatchCount),
        //            PlannedStart = roundStart,
        //            Phase = RoundPhase.Knockout,
        //            Status = RoundStatus.Scheduled
        //        };

        //        for (var matchIndex = 0; matchIndex < roundMatchCount; matchIndex++)
        //        {
        //            var matchStart = round.PlannedStart.AddMinutes(matchIndex * (duration + breakMinutes));
        //            var match = new Match
        //            {
        //                Round = round,
        //                Status = MatchStatus.Scheduled,
        //                PlannedStart = matchStart,
        //                PlannedEnd = matchStart.AddMinutes(duration)
        //            };

        //            if (roundIndex == 0)
        //            {
        //                var slot = firstRoundSlots[matchIndex];
        //                foreach (var player in slot)
        //                {
        //                    match.Participants.Add(new MatchParticipant
        //                    {
        //                        TournamentPlayerId = player.TournamentPlayerId,
        //                        Score = 0,
        //                        IsWinner = slot.Count == 1
        //                    });
        //                }

        //                // A one-player slot is a normal bye. An empty slot is a double-bye
        //                // caused by the power-of-two bracket size; both are administrative
        //                // completed records and never become playable matches.
        //                if (slot.Count <= 1)
        //                {
        //                    match.Status = MatchStatus.Completed;
        //                    match.ActualEnd = matchStart;
        //                }
        //            }

        //            round.Matches.Add(match);
        //        }

        //        round.PlannedEnd = round.PlannedStart.AddMinutes(
        //            Math.Max(0, (roundMatchCount * (duration + breakMinutes)) - breakMinutes));

        //        rounds.Add(round);
        //        roundStart = round.PlannedEnd!.Value.AddMinutes(breakMinutes);
        //        roundMatchCount /= 2;
        //    }

        //    await _context.Rounds.AddRangeAsync(rounds);
        //    await _context.SaveChangesAsync();

        //    // A bye winner is already known. Propagate all such winners through the empty
        //    // bracket slots; if two bye winners meet, the resulting match remains playable.
        //    foreach (var firstRoundMatch in rounds[0].Matches
        //        .OrderBy(m => m.PlannedStart)
        //        .ThenBy(m => m.Id)
        //        .ToList())
        //    {
        //        if (firstRoundMatch.Status == MatchStatus.Completed)
        //            await AdvanceKnockoutAsync(firstRoundMatch.Id);
        //    }

        //    await _context.SaveChangesAsync();
        //}

        //public async Task AdvanceKnockoutAsync(int matchId)
        //{
        //    var match = await _context.Matches
        //        .Include(m => m.Round)
        //        .Include(m => m.Participants)
        //        .FirstOrDefaultAsync(m => m.Id == matchId);

        //    if (match == null || match.Round.Phase != RoundPhase.Knockout || match.Status != MatchStatus.Completed)
        //        return;

        //    var winner = match.Participants.SingleOrDefault(p => p.IsWinner);
        //    if (winner == null)
        //        return;

        //    var nextRound = await _context.Rounds
        //        .Where(r => r.TournamentId == match.Round.TournamentId
        //            && r.Phase == RoundPhase.Knockout
        //            && r.Sequence == match.Round.Sequence + 1)
        //        .Include(r => r.Matches)
        //            .ThenInclude(m => m.Participants)
        //        .FirstOrDefaultAsync();

        //    if (nextRound == null)
        //    {
        //        var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == match.Round.TournamentId);
        //        if (tournament != null)
        //            tournament.Status = TournamentStatus.Completed;
        //        return;
        //    }

        //    var currentRoundMatches = await _context.Matches
        //        .Where(m => m.RoundId == match.RoundId)
        //        .Include(m => m.Participants)
        //        .OrderBy(m => m.PlannedStart)
        //        .ThenBy(m => m.Id)
        //        .ToListAsync();

        //    var currentIndex = currentRoundMatches.FindIndex(m => m.Id == match.Id);
        //    if (currentIndex < 0)
        //        return;

        //    var nextRoundMatches = nextRound.Matches
        //        .OrderBy(m => m.PlannedStart)
        //        .ThenBy(m => m.Id)
        //        .ToList();
        //    var nextMatchIndex = currentIndex / 2;
        //    if (nextMatchIndex >= nextRoundMatches.Count)
        //        throw new InvalidOperationException("Die K.-o.-Baumstruktur ist inkonsistent.");

        //    var nextMatch = nextRoundMatches[nextMatchIndex];
        //    if (nextMatch.Participants.Any(p => p.TournamentPlayerId == winner.TournamentPlayerId))
        //        return;

        //    nextMatch.Participants.Add(new MatchParticipant
        //    {
        //        TournamentPlayerId = winner.TournamentPlayerId,
        //        Score = 0,
        //        IsWinner = false
        //    });

        //    // If both source matches are completed, their winners now form the next match.
        //    // A completed source can have one participant (bye) or two participants (real match).
        //    var siblingIndex = currentIndex % 2 == 0 ? currentIndex + 1 : currentIndex - 1;
        //    var sibling = siblingIndex >= 0 && siblingIndex < currentRoundMatches.Count
        //        ? currentRoundMatches[siblingIndex]
        //        : null;

        //    if (sibling?.Status == MatchStatus.Completed)
        //    {
        //        var siblingWinner = sibling.Participants.SingleOrDefault(p => p.IsWinner);
        //        if (siblingWinner != null
        //            && nextMatch.Participants.All(p => p.TournamentPlayerId != siblingWinner.TournamentPlayerId))
        //        {
        //            nextMatch.Participants.Add(new MatchParticipant
        //            {
        //                TournamentPlayerId = siblingWinner.TournamentPlayerId,
        //                Score = 0,
        //                IsWinner = false
        //            });
        //        }
        //    }

        //    if (nextMatch.Participants.Count == 1
        //        && sibling?.Status == MatchStatus.Completed)
        //    {
        //        nextMatch.Status = MatchStatus.Completed;
        //        nextMatch.Participants.Single().IsWinner = true;
        //        nextMatch.ActualEnd = nextMatch.PlannedStart;
        //        await _context.SaveChangesAsync();
        //        await AdvanceKnockoutAsync(nextMatch.Id);
        //    }
        //}

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

            return result;
        }

        private static List<MatchCandidate> CreateRoundRobinCandidates(Group group, List<int> players)
        {
            var result = new List<MatchCandidate>();
            var rotation = players.Cast<int?>().ToList();
            if (rotation.Count % 2 != 0)
                rotation.Add(null);

            var rounds = rotation.Count - 1;
            for (var round = 0; round < rounds; round++)
            {
                for (var index = 0; index < rotation.Count / 2; index++)
                {
                    var first = rotation[index];
                    var second = rotation[rotation.Count - 1 - index];

                    if (first.HasValue && second.HasValue)
                    {
                        result.Add(new MatchCandidate(group, new[] { first.Value, second.Value }, false));
                    }
                    else if (first.HasValue || second.HasValue)
                    {
                        var player = first ?? second;
                        result.Add(new MatchCandidate(group, new[] { player!.Value }, true));
                    }
                }

                // Keep the first player fixed and rotate all others by one position.
                var last = rotation[^1];
                rotation.RemoveAt(rotation.Count - 1);
                rotation.Insert(1, last);
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
