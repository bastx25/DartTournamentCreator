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

        private readonly IBraketRepository _braketRepo;
        private readonly IBoardService _boardService;

        public MatchMakerService(DartDbContext context,
            ITournamentPlayerRepository tournamentPlayerRepository,
            IMatchRepository matchRepository,
            IGroupRepository groupRepository,
            IBraketRepository braketRepository,
            IBoardService boardService)
        {
            _context = context;
            _tournamentPlayerRepo = tournamentPlayerRepository;
            _matchRepo = matchRepository;
            _groupRepo = groupRepository;
            _braketRepo = braketRepository;
            _boardService = boardService;
        }

        public async Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options)
        {
            ArgumentNullException.ThrowIfNull(options);

            await _groupRepo.DeleteAllTournamentGroups(tournamentId);
            await _braketRepo.DeleteAllTournamentBraketsAsync(tournamentId);

            await _tournamentPlayerRepo.SetTournamentPlayers(tournamentId, options.PlayerIds, options);

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
                || await _context.Brakets.AnyAsync(r => r.TournamentId == tournamentId);
            if (hasGeneratedData)
                throw new InvalidOperationException("Für dieses Turnier wurden bereits Gruppen oder Runden generiert.");
            #endregion Validation

            // Shuffle first, then distribute braket-robin. This gives groups whose sizes differ by at most one.
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

        public async Task SaveManualGroupsAsync(int tournamentId, ManualGroupSetupDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Groups == null || dto.Groups.Count == 0)
                throw new InvalidOperationException("Es muss mindestens eine Gruppe angegeben werden.");

            var allPlayerIds = dto.Groups.SelectMany(g => g.PlayerIds).ToList();

            if (allPlayerIds.Count == 0)
                throw new InvalidOperationException("Es wurden keine Spieler den Gruppen zugeordnet.");

            if (allPlayerIds.Count != allPlayerIds.Distinct().Count())
                throw new InvalidOperationException("Ein Spieler darf nur genau einer Gruppe zugeordnet sein.");

            foreach (var groupDto in dto.Groups)
            {
                if (string.IsNullOrWhiteSpace(groupDto.Name))
                    throw new InvalidOperationException("Jede Gruppe muss einen Namen haben.");

                if (groupDto.PlayerIds.Count < 2)
                    throw new InvalidOperationException($"Gruppe '{groupDto.Name}' muss mindestens 2 Spieler enthalten, um Spiele erstellen zu können.");
            }

            var groupNames = dto.Groups.Select(g => g.Name.Trim()).ToList();
            if (groupNames.Count != groupNames.Distinct(StringComparer.OrdinalIgnoreCase).Count())
                throw new InvalidOperationException("Die Gruppennamen müssen innerhalb des Turniers eindeutig sein.");

            await _groupRepo.DeleteAllTournamentGroups(tournamentId);
            await _braketRepo.DeleteAllTournamentBraketsAsync(tournamentId);

            var genOptions = new GenerateGroupsDto
            {
                StartTime = dto.StartTime,
                MatchDurationMinutes = dto.MatchDurationMinutes,
                BreakBetweenMatchesMinutes = dto.BreakBetweenMatchesMinutes,
                GroupCount = dto.Groups.Count,
                QualifiersPerGroup = dto.Groups.Max(g => g.QualifiersCount),
                PlayerIds = allPlayerIds
            };

            await _tournamentPlayerRepo.SetTournamentPlayers(tournamentId, allPlayerIds, genOptions);

            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPlayers)
                .Include(t => t.Config)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
                throw new KeyNotFoundException($"Turnier {tournamentId} wurde nicht gefunden.");

            var tournamentPlayersMap = tournament.TournamentPlayers.ToDictionary(tp => tp.PlayerId, tp => tp);

            var groups = new List<Group>(dto.Groups.Count);

            for (var index = 0; index < dto.Groups.Count; index++)
            {
                var groupItem = dto.Groups[index];
                var group = new Group
                {
                    TournamentId = tournamentId,
                    Sequence = index + 1,
                    Name = groupItem.Name.Trim(),
                    QualifiersCount = groupItem.QualifiersCount
                };

                foreach (var playerId in groupItem.PlayerIds)
                {
                    if (tournamentPlayersMap.TryGetValue(playerId, out var tp))
                    {
                        group.GroupPlayers.Add(new GroupPlayer
                        {
                            TournamentPlayerId = tp.Id
                        });
                    }
                }

                groups.Add(group);
            }

            var startTime = dto.StartTime ?? tournament.StartDate;
            var matchDuration = dto.MatchDurationMinutes ?? tournament.Config?.MatchDurationMinutes ?? 30;
            var breakMinutes = dto.BreakBetweenMatchesMinutes ?? tournament.Config?.BreakBetweenMatchesMinutes ?? 5;

            ValidateSchedule(startTime, matchDuration, breakMinutes);

            await ScheduleGroupMatches(groups);
            await _boardService.SetBoards(groups, startTime, matchDuration, breakMinutes);

            if (tournament.Config != null)
            {
                tournament.Config.GroupCount = groups.Count;
                tournament.Config.QualifiersPerGroup = groups.Max(g => g.QualifiersCount);
                tournament.Config.MatchDurationMinutes = matchDuration;
                tournament.Config.BreakBetweenMatchesMinutes = breakMinutes;
                tournament.Config.KnockoutDrawMode = dto.KnockoutDrawMode;
            }

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

                var candidates = CreateBraketRobinCandidates(group, players);
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

            // Prüfen, ob noch unvollständige Tiebreaker existieren
            var hasUnfinishedTiebreakers = await _context.Groups
                .AnyAsync(g => g.TournamentId == tournamentId
                    && g.Name.Contains(MatchStatus.Tiebreaker.ToString())
                    && g.Matches.Any(m => m.Status != MatchStatus.Completed));

            if (hasUnfinishedTiebreakers)
            {
                throw new InvalidOperationException("Tiebreak-Spiele sind noch nicht abgeschlossen.");
            }

            if (qualifiedPlayers.Count < 2)
            {
                throw new InvalidOperationException("Nicht genügend qualifizierte Spieler für die K.-o.-Phase vorhanden (mindestens 2 erforderlich).");
            }

            await CreateKnockoutBrakets(tournamentId, options, qualifiedPlayers);
        }

        private static bool IsPowerOf2(int count)
        {
            return count > 0 && (count & (count - 1)) == 0;
        }

        private async Task CreateKnockoutBrakets(int tournamentId, GenerateGroupsDto options, List<TournamentPlayer> qualifiedPlayers)
        {
            await _braketRepo.DeleteAllTournamentBraketsAsync(tournamentId);

            // Nächste Zweierpotenz berechnen (mindestens 2)
            int bracketSize = 2;
            while (bracketSize < qualifiedPlayers.Count)
            {
                bracketSize *= 2;
            }

            var brakets = CreateEmptyBrakets(tournamentId, bracketSize);

            // Spieler sortieren je nach gewähltem KnockoutDrawMode:
            List<TournamentPlayer> orderedPlayers;

            if (options.KnockoutDrawMode == KnockoutDrawMode.RankedSeeding)
            {
                // Ergebnisse und Gruppenplatzierungen aller Gruppen laden
                var groups = await _context.Groups
                    .Where(g => g.TournamentId == tournamentId)
                    .Include(g => g.Matches)
                    .ToListAsync();

                var groupResults = await GetGroupResults(groups);
                var playerStatsMap = groupResults
                    .SelectMany(g => g.Players)
                    .ToDictionary(p => p.Player.Id, p => p);

                // Gruppenübergreifende Rangliste:
                // 1. Gruppenrang (Gruppenerste vor Gruppenzweiten usw.)
                // 2. Siegquote (WinRate, fair bei unterschiedlicher Gruppengröße)
                // 3. Absolute Siege
                // 4. Leg-Differenz pro Match
                // 5. Absolute Leg-Differenz
                // 6. Gewonnene Legs
                // 7. Stabile Spieler-ID
                orderedPlayers = qualifiedPlayers
                    .Select(p => playerStatsMap.TryGetValue(p.Id, out var s) ? s : new GroupPlayerResult(p, 0))
                    .OrderBy(s => s.GroupRank > 0 ? s.GroupRank : 999)
                    .ThenByDescending(s => s.WinRate)
                    .ThenByDescending(s => s.Wins)
                    .ThenByDescending(s => s.LegDiffPerMatch)
                    .ThenByDescending(s => s.LegDiff)
                    .ThenByDescending(s => s.LegsWon)
                    .ThenBy(s => s.Player.Id)
                    .Select(s => s.Player)
                    .ToList();
            }
            else
            {
                // Zufällige Auslosung (Random)
                orderedPlayers = qualifiedPlayers.OrderBy(_ => _random.Next()).ToList();
            }

            var pairings = GenerateBracketSeedPairings(bracketSize);
            var firstBraket = brakets[0];
            var firstRoundMatches = new List<Match>();
            var byeMatches = new List<Match>();

            for (int i = 0; i < pairings.Count; i++)
            {
                var (s1, s2) = pairings[i];
                var p1 = s1 <= orderedPlayers.Count ? orderedPlayers[s1 - 1] : null;
                var p2 = s2 <= orderedPlayers.Count ? orderedPlayers[s2 - 1] : null;

                if (p1 != null && p2 != null)
                {
                    firstRoundMatches.Add(new Match
                    {
                        BraketId = firstBraket.Id,
                        Braket = firstBraket,
                        BoardId = 1,
                        Status = MatchStatus.Scheduled,
                        Participants = new List<MatchParticipant>
                        {
                            new MatchParticipant
                            {
                                TournamentPlayerId = p1.Id,
                                TournamentPlayer = p1,
                                Score = 0,
                                IsWinner = false
                            },
                            new MatchParticipant
                            {
                                TournamentPlayerId = p2.Id,
                                TournamentPlayer = p2,
                                Score = 0,
                                IsWinner = false
                            }
                        }
                    });
                }
                else if (p1 != null || p2 != null)
                {
                    var byePlayer = p1 ?? p2!;
                    var byeMatch = new Match
                    {
                        BraketId = firstBraket.Id,
                        Braket = firstBraket,
                        BoardId = 1,
                        Status = MatchStatus.Completed,
                        Participants = new List<MatchParticipant>
                        {
                            new MatchParticipant
                            {
                                TournamentPlayerId = byePlayer.Id,
                                TournamentPlayer = byePlayer,
                                Score = 0,
                                IsWinner = true
                            }
                        }
                    };
                    firstRoundMatches.Add(byeMatch);
                    byeMatches.Add(byeMatch);
                }
            }
            firstBraket.Matches = firstRoundMatches;

            // Weitere K.-o.-Runden mit leeren Matches initialisieren
            for (int b = 1; b < brakets.Count; b++)
            {
                var braket = brakets[b];
                var matches = new List<Match>();
                for (int i = 0; i < braket.Sequence; i++)
                {
                    matches.Add(new Match
                    {
                        BraketId = braket.Id,
                        Braket = braket,
                        BoardId = 1,
                        Status = MatchStatus.Scheduled
                    });
                }
                braket.Matches = matches;
            }

            await _boardService.SetBoards(
                brakets,
                options.StartTime ?? DateTimeOffset.UtcNow,
                options.MatchDurationMinutes ?? 30,
                options.BreakBetweenMatchesMinutes ?? 5);

            await _context.Brakets.AddRangeAsync(brakets);
            await _context.SaveChangesAsync();

            // Freilose (Byes) automatisch in die nächste Runde vorrücken
            foreach (var byeMatch in byeMatches)
            {
                await AdvanceWinner(byeMatch);
            }
        }

        public static List<(int Seed1, int Seed2)> GenerateBracketSeedPairings(int bracketSize)
        {
            if (bracketSize < 2) return new List<(int, int)>();

            var round = new List<int> { 1, 2 };

            while (round.Count < bracketSize)
            {
                int nextSize = round.Count * 2;
                var nextRound = new List<int>(nextSize);
                for (int i = 0; i < round.Count; i++)
                {
                    nextRound.Add(round[i]);
                    nextRound.Add(nextSize + 1 - round[i]);
                }
                round = nextRound;
            }

            var pairings = new List<(int Seed1, int Seed2)>();
            for (int i = 0; i < round.Count; i += 2)
            {
                pairings.Add((round[i], round[i + 1]));
            }
            return pairings;
        }

        private List<Braket> CreateEmptyBrakets(int tournamentId, int count)
        {
            var brakets = new List<Braket>();

            for (int i = count; i >= 2; i /= 2)
            {
                brakets.Add(new Braket
                {
                    TournamentId = tournamentId,
                    Sequence = i/2,
                    Name = GetKoGroupName(i),
                    Phase = BraketPhase.Knockout
                });
            }

            return brakets;
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

            await _braketRepo.DeleteAllTournamentBraketsAsync(tournamentId);

            //var groups = await _groupRepo.GetGroupsAsync(tournamentId);
            var groups = await _context.Groups.Where(g => g.TournamentId == tournamentId).Include(g => g.Matches).ToListAsync();

            var groupPhaseEnd = groups
                    .SelectMany(g => g.Matches)
                    .Select(m => m.PlannedEnd)
                    .Max();

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

            
            var matchDuration =
                options.MatchDurationMinutes ??
                tournament.Config.MatchDurationMinutes;

            var breakMinutes =
                options.BreakBetweenMatchesMinutes ??
                tournament.Config.BreakBetweenMatchesMinutes;

            options.StartTime = groupPhaseEnd?.AddMinutes(breakMinutes);
            var startTime = options.StartTime ?? tournament.StartDate;

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

            options.StartTime = startTime.AddMinutes(matchDuration).AddMinutes(breakMinutes);

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
            //TODO: Map Time and Board to Tiebreakers
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

                var matches = (await _matchRepo.GetByGroupIdAsync(group.Id)).ToList();

                // Alle Spieler der Gruppe ermitteln
                var groupPlayers = await _context.GroupPlayers
                    .Where(gp => gp.GroupId == group.Id)
                    .Include(gp => gp.TournamentPlayer)
                        .ThenInclude(tp => tp.Player)
                    .Select(gp => gp.TournamentPlayer)
                    .ToListAsync();

                // Falls noch keine GroupPlayers explizit gespeichert sind, aus Match-Teilnehmern ermitteln
                if (!groupPlayers.Any())
                {
                    groupPlayers = matches
                        .SelectMany(m => m.Participants)
                        .Where(p => p.TournamentPlayer != null)
                        .Select(p => p.TournamentPlayer)
                        .GroupBy(tp => tp.Id)
                        .Select(g => g.First())
                        .ToList();
                }

                var playerStats = new List<GroupPlayerResult>();

                foreach (var player in groupPlayers)
                {
                    var playerMatches = matches
                        .Where(m => m.Participants.Any(p => p.TournamentPlayerId == player.Id))
                        .ToList();

                    int wins = 0;
                    int legsWon = 0;
                    int legsLost = 0;
                    int matchesPlayed = 0;

                    foreach (var match in playerMatches)
                    {
                        var myPart = match.Participants.FirstOrDefault(p => p.TournamentPlayerId == player.Id);
                        var oppPart = match.Participants.FirstOrDefault(p => p.TournamentPlayerId != player.Id);

                        if (myPart == null) continue;

                        if (match.Status == MatchStatus.Completed || myPart.Score > 0 || (oppPart?.Score ?? 0) > 0 || myPart.IsWinner)
                        {
                            matchesPlayed++;
                        }

                        if (myPart.IsWinner)
                        {
                            wins++;
                        }

                        legsWon += myPart.Score;
                        if (oppPart != null)
                        {
                            legsLost += oppPart.Score;
                        }
                    }

                    playerStats.Add(new GroupPlayerResult(
                        player,
                        wins,
                        legsWon,
                        legsLost,
                        matchesPlayed,
                        group.Id));
                }

                // Sortierung innerhalb der Gruppe:
                // 1. Siege
                // 2. Leg-Differenz
                // 3. Gewonnene Legs
                // 4. Stabile Spieler-ID
                var sortedStats = playerStats
                    .OrderByDescending(x => x.Wins)
                    .ThenByDescending(x => x.LegDiff)
                    .ThenByDescending(x => x.LegsWon)
                    .ThenBy(x => x.Player.Id)
                    .Select((p, idx) => p with { GroupRank = idx + 1 })
                    .ToList();

                results.Add(new GroupResult(group.Id, sortedStats));
            }

            return results;
        }

        public sealed record GroupResult(
            int GroupId,
            List<GroupPlayerResult> Players);

        public sealed record GroupPlayerResult(
            TournamentPlayer Player,
            int Wins,
            int LegsWon = 0,
            int LegsLost = 0,
            int MatchesPlayed = 0,
            int GroupId = 0,
            int GroupRank = 0)
        {
            public int LegDiff => LegsWon - LegsLost;
            public double WinRate => MatchesPlayed > 0 ? (double)Wins / MatchesPlayed : 0;
            public double LegDiffPerMatch => MatchesPlayed > 0 ? (double)LegDiff / MatchesPlayed : 0;
        }

        private static bool IsStrictlyBetter(GroupPlayerResult a, GroupPlayerResult b)
        {
            if (a.Wins != b.Wins) return a.Wins > b.Wins;
            if (a.LegDiff != b.LegDiff) return a.LegDiff > b.LegDiff;
            return a.LegsWon > b.LegsWon;
        }

        private static bool IsEqualScore(GroupPlayerResult a, GroupPlayerResult b)
        {
            return a.Wins == b.Wins && a.LegDiff == b.LegDiff && a.LegsWon == b.LegsWon;
        }

        private (List<TournamentPlayer> KnockoutPlayers, List<TournamentPlayer> TiebreakPlayers) GetKnockoutPlayers(
            IEnumerable<GroupResult> groups,
            int qualifiersPerGroup)
        {
            var knockoutPlayers = new List<TournamentPlayer>();
            var tiebreakPlayers = new List<TournamentPlayer>();

            foreach (var group in groups)
            {
                var players = group.Players;
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

                var cutoffPlayer = qualifiedCandidates.Last();

                // Spieler mit strikt besseren Werten sind fix qualifiziert
                var fixedQualified = players
                    .Where(x => IsStrictlyBetter(x, cutoffPlayer))
                    .ToList();

                knockoutPlayers.AddRange(
                    fixedQualified.Select(x => x.Player));

                // Spieler mit exakt gleichen Werten kämpfen um die verbleibenden Qualifikationsplätze
                var tiedPlayers = players
                    .Where(x => IsEqualScore(x, cutoffPlayer))
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

        public async Task AdvanceKnockoutAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Braket)
                    .ThenInclude(r => r!.Tournament)
                .Include(m => m.Participants)
                    .ThenInclude(p => p.TournamentPlayer)
                        .ThenInclude(tp => tp.Player)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match != null && match.Status == MatchStatus.Completed && match.Participants.Any(p => p.IsWinner))
            {
                await AdvanceWinner(match);
            }
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


        private static List<MatchCandidate> CreateBraketRobinCandidates(
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

        public async Task AdvanceWinner(Match match)
        {
            var braket = match.Braket ?? await _context.Brakets.FirstOrDefaultAsync(b => b.Id == match.BraketId);
            if (braket == null || braket.Phase != BraketPhase.Knockout)
            {
                return;
            }

            var winner = match.Participants.FirstOrDefault(p => p.IsWinner);
            if (winner == null) return;

            var brakets = await _context.Brakets
                .Where(b => b.TournamentId == braket.TournamentId)
                .Include(b => b.Matches)
                    .ThenInclude(m => m.Participants)
                .ToListAsync();

            var curBraket = brakets.FirstOrDefault(b => b.Id == braket.Id);
            if (curBraket == null) return;

            var nextBraket = brakets.OrderByDescending(b => b.Sequence).FirstOrDefault(b => b.Sequence < curBraket.Sequence);
            if (nextBraket == null) return;

            var matchIndex = curBraket.Matches
                .OrderBy(m => m.Id)
                .ToList()
                .FindIndex(m => m.Id == match.Id);

            if (matchIndex < 0) return;

            var targetMatchIndex = matchIndex / 2;
            var orderedNextMatches = nextBraket.Matches.OrderBy(m => m.Id).ToList();
            if (targetMatchIndex >= orderedNextMatches.Count) return;

            var nextMatch = orderedNextMatches[targetMatchIndex];

            // Prevent adding duplicate winner
            if (nextMatch.Participants.Any(p => p.TournamentPlayerId == winner.TournamentPlayerId))
            {
                return;
            }

            if (nextMatch.Participants.Count < 2)
            {
                nextMatch.Participants.Add(new MatchParticipant
                {
                    Match = nextMatch,
                    MatchId = nextMatch.Id,
                    TournamentPlayer = winner.TournamentPlayer,
                    TournamentPlayerId = winner.TournamentPlayerId,
                    Score = 0,
                    IsWinner = false
                });

                await _context.SaveChangesAsync();
            }
        }

        private sealed record MatchCandidate(Group Group, IReadOnlyList<int> PlayerIds, bool IsBye);

    }
}
