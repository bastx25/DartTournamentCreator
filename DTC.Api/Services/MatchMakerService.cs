using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Enums;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DTC.Api.Services
{
    public class MatchMakerService : IMatchMakerService
    {
        private readonly DartDbContext _context;

        public MatchMakerService(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
        }

        public async Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options)
        {
            var tournament = await _context.Tournaments.FindAsync(tournamentId);
            if (tournament == null)
                throw new KeyNotFoundException($"Turnier {tournamentId} wurde nicht gefunden.");

            ValidateTournamentModeForGroupGeneration(tournament);
            ValidateGenerationOptions(options);

            var alreadyGenerated = await _context.Rounds.AnyAsync(r => r.TournamentId == tournamentId)
                || await _context.Groups.AnyAsync(g => g.TournamentId == tournamentId);
            if (alreadyGenerated)
                await DeleteGeneratedTournamentPhaseAsync(tournament.Id);
            //throw new InvalidOperationException("Die Gruppenphase wurde für dieses Turnier bereits generiert und kann nicht erneut erstellt werden.");

            var players = await LoadPlayersAsync(options.PlayerIds);
            //ValidatePlayers(players, options.GroupCount, options.GroupSize);

            var locationAndBoards = await FindLocationWithBoardsAsync(options.GroupCount);
            if (locationAndBoards == null)
            {
                throw new InvalidOperationException(
                    $"Für {options.GroupCount} Gruppen werden mindestens {options.GroupCount} aktive Boards am selben Standort benötigt.");
            }

            var startTime = options.StartTime ?? tournament.StartDate;
            var matchDuration = options.MatchDurationMinutes ?? tournament.MatchDurationMinutes;
            var breakDuration = options.BreakBetweenMatchesMinutes ?? tournament.BreakBetweenMatchesMinutes;
            ValidateTiming(matchDuration, breakDuration);

            tournament.MatchDurationMinutes = matchDuration;
            tournament.BreakBetweenMatchesMinutes = breakDuration;

            var groups = BuildGroups(tournament, players, options, locationAndBoards.Value.boards);
            _context.Groups.AddRange(groups);

            var groupRounds = BuildGroupStageRounds(
                tournament,
                groups,
                locationAndBoards.Value.locationId,
                startTime,
                matchDuration,
                breakDuration);
            _context.Rounds.AddRange(groupRounds);

            if (tournament.Mode == TournamentMode.GrouStageandKnockout)
            {
                var qualifiersTotal = groups.Sum(g => g.QualifiersCount);
                if (qualifiersTotal < 2)
                    throw new InvalidOperationException("Für die K.-o.-Phase müssen mindestens zwei Spieler qualifiziert werden.");

                if (!IsPowerOfTwo(qualifiersTotal))
                    throw new InvalidOperationException(
                        $"Die Summe der Weiterkommer muss eine Zweierpotenz sein (2, 4, 8, 16, ...). Aktuell sind {qualifiersTotal} konfiguriert.");

                BuildEmptyKnockoutRounds(
                    tournament,
                    groups,
                    groupRounds,
                    locationAndBoards.Value.locationId,
                    locationAndBoards.Value.boards,
                    matchDuration,
                    breakDuration);
            }

            await _context.SaveChangesAsync();
        }


        private async Task DeleteGeneratedTournamentPhaseAsync(int tournamentId)
        {
            var groupIds = await _context.Groups
                .Where(g => g.TournamentId == tournamentId)
                .Select(g => g.Id)
                .ToListAsync();

            var roundIds = await _context.Rounds
                .Where(r => r.TournamentId == tournamentId)
                .Select(r => r.Id)
                .ToListAsync();

            var matchIds = await _context.Matches
                .Where(m =>
                    (m.GroupId.HasValue && groupIds.Contains(m.GroupId.Value)) ||
                    roundIds.Contains(m.RoundId))
                .Select(m => m.Id)
                .ToListAsync();

            await _context.MatchParticipants
                .Where(mp => matchIds.Contains(mp.MatchId))
                .ExecuteDeleteAsync();

            await _context.Matches
                .Where(m => matchIds.Contains(m.Id))
                .ExecuteDeleteAsync();

            await _context.GroupPlayers
                .Where(gp => groupIds.Contains(gp.GroupId))
                .ExecuteDeleteAsync();

            await _context.Groups
                .Where(g => g.TournamentId == tournamentId)
                .ExecuteDeleteAsync();

            await _context.Rounds
                .Where(r => r.TournamentId == tournamentId)
                .ExecuteDeleteAsync();
        }


        public async Task GenerateKnockoutAsync(int tournamentId)
        {
            var tournament = await _context.Tournaments.FindAsync(tournamentId);
            if (tournament == null)
                throw new KeyNotFoundException($"Turnier {tournamentId} wurde nicht gefunden.");

            if (tournament.Mode != TournamentMode.GrouStageandKnockout)
                throw new InvalidOperationException("Dieses Turnier ist nicht für eine K.-o.-Phase konfiguriert.");

            var groups = await _context.Groups
                .Where(g => g.TournamentId == tournamentId)
                .Include(g => g.Players)
                .ThenInclude(gp => gp.Player)
                .OrderBy(g => g.Sequence)
                .ToListAsync();

            if (groups.Count == 0)
                throw new InvalidOperationException("Die Gruppenphase wurde noch nicht generiert.");

            var qualifiersTotal = groups.Sum(g => g.QualifiersCount);
            if (!IsPowerOfTwo(qualifiersTotal))
                throw new InvalidOperationException(
                    $"Die Summe der Weiterkommer muss eine Zweierpotenz sein. Aktuell sind {qualifiersTotal} konfiguriert.");

            var rounds = await _context.Rounds
                .Where(r => r.TournamentId == tournamentId && r.Phase == RoundPhase.Knockout)
                .OrderBy(r => r.Sequence)
                .Include(r => r.Matches)
                    .ThenInclude(m => m.Participants)
                .ToListAsync();

            var qualificationRound = rounds
                .FirstOrDefault(r => r.Name == "Ausspielrunde");

            var knockoutRounds = rounds
                .Where(r => r.Name != "Ausspielrunde")
                .OrderBy(r => r.Sequence)
                .ToList();

            if (knockoutRounds.Count == 0)
                throw new InvalidOperationException("Für dieses Turnier wurden noch keine K.-o.-Slots vorbereitet.");

            var firstRound = knockoutRounds.First();
            var firstRoundMatches = firstRound.Matches.OrderBy(m => m.Id).ToList();

            if (firstRoundMatches.Count * 2 < qualifiersTotal)
                throw new InvalidOperationException("Die vorbereiteten K.-o.-Slots reichen für die qualifizierten Spieler nicht aus.");

            if (firstRoundMatches.Any(m => m.Participants.Count > 0))
                throw new InvalidOperationException("Die K.-o.-Phase wurde bereits generiert und kann nicht erneut generiert werden.");

            var groupMatches = await _context.Matches
                .Where(m => m.Round.TournamentId == tournamentId && m.Round.Phase == RoundPhase.GroupStage)
                .Include(m => m.Participants)
                .ToListAsync();

            groupMatches.RemoveAll(m => m.GroupId == null);

            if (groupMatches.Count == 0 || groupMatches.Any(m => m.Status != MatchStatus.Completed))
            {
                throw new InvalidOperationException(
                    "Die K.-o.-Phase kann erst generiert werden, wenn alle Gruppenphasen-Matches abgeschlossen sind.");
            }

            if (groupMatches.Any(m => m.Participants.Count(p => p.IsWinner) != 1))
            {
                throw new InvalidOperationException(
                    "Die K.-o.-Phase kann erst generiert werden, wenn jedes Gruppenphasen-Match genau einen Sieger enthält.");
            }

            var winsByPlayer = groupMatches
                .SelectMany(m => m.Participants.Where(p => p.IsWinner))
                .GroupBy(p => p.PlayerId)
                .ToDictionary(g => g.Key, g => g.Count());

            var playoffWinnersByGroup = new Dictionary<int, List<int>>();
            if (qualificationRound != null)
            {
                var playoffMatches = qualificationRound.Matches.OrderBy(m => m.Id).ToList();

                if (playoffMatches.Any(m => m.Status != MatchStatus.Completed || m.Participants.Count(p => p.IsWinner) != 1))
                {
                    throw new InvalidOperationException(
                        "Die Ausspielrunde muss abgeschlossen sein, bevor die K.-o.-Phase generiert werden kann.");
                }

                foreach (var match in playoffMatches)
                {
                    if (!match.GroupId.HasValue)
                        continue;

                    var winner = match.Participants.Single(p => p.IsWinner).PlayerId;
                    if (!playoffWinnersByGroup.TryGetValue(match.GroupId.Value, out var winners))
                    {
                        winners = new List<int>();
                        playoffWinnersByGroup[match.GroupId.Value] = winners;
                    }

                    winners.Add(winner);
                }
            }

            var qualifiedByGroup = new List<List<int>>();
            var playoffMatchesToCreate = new List<(Group Group, int Player1Id, int Player2Id)>();

            foreach (var group in groups)
            {
                var rankedPlayers = group.Players
                    .Select(gp => new
                    {
                        gp.PlayerId,
                        Wins = winsByPlayer.TryGetValue(gp.PlayerId, out var wins) ? wins : 0
                    })
                    .OrderByDescending(x => x.Wins)
                    .ThenBy(x => x.PlayerId)
                    .ToList();

                if (rankedPlayers.Count < group.QualifiersCount)
                {
                    throw new InvalidOperationException(
                        $"In {group.Name} gibt es weniger Spieler als konfigurierte Weiterkommer.");
                }

                var cutoffWins = rankedPlayers[group.QualifiersCount - 1].Wins;
                var fixedPlayers = rankedPlayers
                    .Where(x => x.Wins > cutoffWins)
                    .Select(x => x.PlayerId)
                    .ToList();

                var cutoffPlayers = rankedPlayers
                    .Where(x => x.Wins == cutoffWins)
                    .Select(x => x.PlayerId)
                    .ToList();

                var slotsToFill = group.QualifiersCount - fixedPlayers.Count;

                playoffWinnersByGroup.TryGetValue(group.Id, out var playoffWinners);
                playoffWinners ??= new List<int>();

                if (playoffWinners.Count > 0)
                {
                    if (playoffWinners.Count != slotsToFill)
                    {
                        throw new InvalidOperationException(
                            $"Die Ausspielrunde für {group.Name} liefert nicht genau {slotsToFill} Sieger.");
                    }

                    var result = fixedPlayers.Concat(playoffWinners).Distinct().ToList();
                    if (result.Count != group.QualifiersCount)
                        throw new InvalidOperationException($"Die Qualifikation für {group.Name} ist nicht eindeutig.");

                    qualifiedByGroup.Add(result);
                    continue;
                }

                if (cutoffPlayers.Count > slotsToFill)
                {
                    if (slotsToFill != 1 || cutoffPlayers.Count != 2)
                    {
                        throw new InvalidOperationException(
                            $"In {group.Name} gibt es einen Gleichstand um den letzten Qualifikationsplatz mit {cutoffPlayers.Count} Spielern. " +
                            "Aktuell wird die automatische Ausspielrunde nur für einen direkten 1-gegen-1-Gleichstand unterstützt.");
                    }

                    playoffMatchesToCreate.Add((group, cutoffPlayers[0], cutoffPlayers[1]));
                    qualifiedByGroup.Add(fixedPlayers);
                    continue;
                }

                qualifiedByGroup.Add(
                    rankedPlayers
                        .Take(group.QualifiersCount)
                        .Select(x => x.PlayerId)
                        .ToList());
            }

            if (playoffMatchesToCreate.Count > 0)
            {
                if (qualificationRound != null)
                    throw new InvalidOperationException("Für dieses Turnier existiert bereits eine Ausspielrunde.");

                qualificationRound = await CreateQualificationRoundAsync(
                    tournament,
                    firstRound,
                    playoffMatchesToCreate);

                await _context.SaveChangesAsync();

                return;
            }

            var qualifiedPlayerIds = qualifiedByGroup.SelectMany(x => x).Distinct().ToList();
            if (qualifiedPlayerIds.Count != qualifiersTotal)
            {
                throw new InvalidOperationException(
                    $"Es wurden {qualifiedPlayerIds.Count} Spieler qualifiziert, erwartet werden {qualifiersTotal}.");
            }

            var seedList = BuildSeedList(qualifiedByGroup);

            for (var i = 0; i < firstRoundMatches.Count; i++)
            {
                var match = firstRoundMatches[i];
                match.Participants.Clear();

                var firstPlayerId = i * 2 < seedList.Count ? seedList[i * 2] : null;
                var secondPlayerId = i * 2 + 1 < seedList.Count ? seedList[i * 2 + 1] : null;

                if (firstPlayerId.HasValue)
                    match.Participants.Add(new MatchParticipant { PlayerId = firstPlayerId.Value });
                if (secondPlayerId.HasValue)
                    match.Participants.Add(new MatchParticipant { PlayerId = secondPlayerId.Value });

                if (match.Participants.Count == 1)
                {
                    match.Status = MatchStatus.Completed;
                    match.Participants.Single().IsWinner = true;
                    match.Participants.Single().Score = 0;
                }
                else
                {
                    match.Status = MatchStatus.Scheduled;
                }
            }

            await _context.SaveChangesAsync();
            await PropagatePreparedByesAsync(tournamentId);
            await _context.SaveChangesAsync();
        }

        private async Task<Round> CreateQualificationRoundAsync(
            Tournament tournament,
            Round firstKnockoutRound,
            IReadOnlyList<(Group Group, int Player1Id, int Player2Id)> playoffMatches)
        {
            var duration = tournament.MatchDurationMinutes;
            var breakDuration = tournament.BreakBetweenMatchesMinutes;

            var boards = await _context.Boards
                .Where(b => b.LocationId == firstKnockoutRound.LocationId && b.IsActive)
                .OrderBy(b => b.Number)
                .ToListAsync();

            if (boards.Count == 0)
                throw new InvalidOperationException("Für die Ausspielrunde ist kein aktives Board verfügbar.");

            var playoffSlots = (int)Math.Ceiling(playoffMatches.Count / (double)boards.Count);
            var shiftMinutes = playoffSlots * (duration + breakDuration);

            var roundsToShift = await _context.Rounds
                .Where(r => r.TournamentId == tournament.Id
                    && r.Phase == RoundPhase.Knockout
                    && r.Sequence >= firstKnockoutRound.Sequence)
                .Include(r => r.Matches)
                .OrderByDescending(r => r.Sequence)
                .ToListAsync();

            foreach (var shiftround in roundsToShift)
            {
                shiftround.Sequence += 1;
                shiftround.PlannedStart = shiftround.PlannedStart.AddMinutes(shiftMinutes);
                if (shiftround.PlannedEnd.HasValue)
                    shiftround.PlannedEnd = shiftround.PlannedEnd.Value.AddMinutes(shiftMinutes);

                foreach (var match in shiftround.Matches)
                {
                    match.PlannedStart = match.PlannedStart?.AddMinutes(shiftMinutes);
                    match.PlannedEnd = match.PlannedEnd?.AddMinutes(shiftMinutes);
                }
            }

            var plannedStart = firstKnockoutRound.PlannedStart;
            var round = new Round
            {
                TournamentId = tournament.Id,
                LocationId = firstKnockoutRound.LocationId,
                Sequence = firstKnockoutRound.Sequence,
                Name = "Ausspielrunde",
                PlannedStart = plannedStart,
                Status = RoundStatus.Scheduled,
                Phase = RoundPhase.Knockout
            };

            for (var i = 0; i < playoffMatches.Count; i++)
            {
                var (group, player1Id, player2Id) = playoffMatches[i];
                var slot = i / boards.Count;
                var plannedMatchStart = plannedStart.AddMinutes(
                    slot * (duration + breakDuration));

                var match = new Match
                {
                    Round = round,
                    Group = group,
                    GroupId = group.Id,
                    BoardId = boards[i % boards.Count].Id,
                    Status = MatchStatus.Scheduled,
                    PlannedStart = plannedMatchStart,
                    PlannedEnd = plannedMatchStart.AddMinutes(duration)
                };

                match.Participants.Add(new MatchParticipant { PlayerId = player1Id });
                match.Participants.Add(new MatchParticipant { PlayerId = player2Id });
                round.Matches.Add(match);
            }

            round.PlannedEnd = round.Matches.Count == 0
                ? round.PlannedStart
                : round.Matches.Max(m => m.PlannedEnd);

            tournament.Rounds.Add(round);
            return round;
        }

        private async Task PropagatePreparedByesAsync(int tournamentId)
        {
            var rounds = await _context.Rounds
                .Where(r => r.TournamentId == tournamentId && r.Phase == RoundPhase.Knockout)
                .OrderBy(r => r.Sequence)
                .Include(r => r.Matches)
                    .ThenInclude(m => m.Participants)
                .ToListAsync();

            for (var roundIndex = 0; roundIndex < rounds.Count - 1; roundIndex++)
            {
                var currentMatches = rounds[roundIndex].Matches.OrderBy(m => m.Id).ToList();
                var nextMatches = rounds[roundIndex + 1].Matches.OrderBy(m => m.Id).ToList();

                for (var i = 0; i < currentMatches.Count; i++)
                {
                    var current = currentMatches[i];
                    if (current.Status != MatchStatus.Completed || current.Participants.Count != 1)
                        continue;

                    var nextMatch = nextMatches[i / 2];
                    var playerId = current.Participants.Single().PlayerId;
                    if (nextMatch.Participants.Any(p => p.PlayerId == playerId))
                        continue;

                    if (nextMatch.Participants.Count < 2)
                    {
                        nextMatch.Participants.Add(new MatchParticipant { PlayerId = playerId });

                        if (nextMatch.Participants.Count == 1)
                        {
                            nextMatch.Status = MatchStatus.Completed;
                            nextMatch.Participants.Single().IsWinner = true;
                            nextMatch.Participants.Single().Score = 0;
                        }
                        else
                        {
                            nextMatch.Status = MatchStatus.Scheduled;
                            foreach (var participant in nextMatch.Participants)
                                participant.IsWinner = false;
                        }
                    }
                }
            }
        }

        private static List<int?> BuildSeedList(List<List<int>> qualifiedByGroup)
        {
            var seeds = new List<int?>();
            var reverse = false;
            var maxQualifiers = qualifiedByGroup.Count == 0 ? 0 : qualifiedByGroup.Max(g => g.Count);

            for (var seed = 0; seed < maxQualifiers; seed++)
            {
                var indices = Enumerable.Range(0, qualifiedByGroup.Count);
                if (reverse)
                    indices = indices.Reverse();

                foreach (var groupIndex in indices)
                {
                    if (seed < qualifiedByGroup[groupIndex].Count)
                        seeds.Add(qualifiedByGroup[groupIndex][seed]);
                }

                reverse = !reverse;
            }

            var bracketSize = NextPowerOfTwo(seeds.Count);
            while (seeds.Count < bracketSize)
                seeds.Add(null);

            return seeds;
        }

        private static bool IsPowerOfTwo(int value) => value > 0 && (value & (value - 1)) == 0;

        private static int NextPowerOfTwo(int value)
        {
            var result = 1;
            while (result < value)
                result *= 2;
            return result;
        }

        private static void BuildEmptyKnockoutRounds(
            Tournament tournament,
            IReadOnlyList<Group> groups,
            List<Round> groupRounds,
            int locationId,
            IReadOnlyList<Board> boards,
            int matchDurationMinutes,
            int breakBetweenMatchesMinutes)
        {
            var qualifiersTotal = groups.Sum(g => g.QualifiersCount);

            if (qualifiersTotal < 2)
                return;

            var bracketSize = NextPowerOfTwo(qualifiersTotal);
            var roundsCount = (int)Math.Log2(bracketSize);
            var phaseStart = groupRounds.Max(r => r.PlannedEnd ?? r.PlannedStart)
                .AddMinutes(breakBetweenMatchesMinutes);
            var nextSequence = groupRounds.Max(r => r.Sequence) + 1;
            var previousEnd = phaseStart;

            for (var roundIndex = 0; roundIndex < roundsCount; roundIndex++)
            {
                var matchesInRound = bracketSize / (int)Math.Pow(2, roundIndex + 1);
                var round = new Round
                {
                    TournamentId = tournament.Id,
                    LocationId = locationId,
                    Sequence = nextSequence++,
                    Name = GetKnockoutRoundName(matchesInRound),
                    PlannedStart = previousEnd,
                    Status = RoundStatus.Scheduled,
                    Phase = RoundPhase.Knockout
                };

                for (var matchIndex = 0; matchIndex < matchesInRound; matchIndex++)
                {
                    var slot = matchIndex / Math.Max(1, boards.Count);
                    var plannedStart = round.PlannedStart
                        .AddMinutes(slot * (matchDurationMinutes + breakBetweenMatchesMinutes));
                    var board = boards[matchIndex % boards.Count];
                    round.Matches.Add(new Match
                    {
                        Round = round,
                        BoardId = board.Id,
                        Status = MatchStatus.Scheduled,
                        PlannedStart = plannedStart,
                        PlannedEnd = plannedStart.AddMinutes(matchDurationMinutes)
                    });
                }

                round.PlannedEnd = round.Matches.Count == 0
                    ? round.PlannedStart
                    : round.Matches.Max(m => m.PlannedEnd);

                tournament.Rounds.Add(round);
                previousEnd = round.PlannedEnd.Value.AddMinutes(breakBetweenMatchesMinutes);
            }
        }

        private static string GetKnockoutRoundName(int matchesInRound) => matchesInRound switch
        {
            1 => "Finale",
            2 => "Halbfinale",
            4 => "Viertelfinale",
            8 => "Achtelfinale",
            _ => $"K.-o.-Runde ({matchesInRound * 2} Spieler)"
        };

        private static List<Round> BuildGroupStageRounds(
            Tournament tournament,
            IReadOnlyList<Group> groups,
            int locationId,
            DateTimeOffset startTime,
            int matchDurationMinutes,
            int breakBetweenMatchesMinutes)
        {
            // Each group has at least one board. Therefore at most one match of a
            // group may occupy that board per time slot. Round-robin pairings are
            // flattened so that the two matches of a four-player round-robin
            // cycle are scheduled one after the other on the group's board.
            var schedules = groups
                .Select(g => CreateRoundRobinSchedule(g.Players.Select(p => p.PlayerId).ToList())
                    .SelectMany(r => r)
                    .ToList())
                .ToList();

            var totalSlots = schedules.Count == 0 ? 0 : schedules.Max(s => s.Count);
            var rounds = new List<Round>();

            for (var slotIndex = 0; slotIndex < totalSlots; slotIndex++)
            {
                var plannedStart = startTime.AddMinutes(
                    slotIndex * (matchDurationMinutes + breakBetweenMatchesMinutes));
                var round = new Round
                {
                    TournamentId = tournament.Id,
                    LocationId = locationId,
                    Sequence = slotIndex + 1,
                    Name = $"Gruppenphase – Runde {slotIndex + 1}",
                    PlannedStart = plannedStart,
                    Status = RoundStatus.Scheduled,
                    Phase = RoundPhase.GroupStage
                };

                for (var groupIndex = 0; groupIndex < groups.Count; groupIndex++)
                {
                    if (slotIndex >= schedules[groupIndex].Count)
                        continue;

                    var pairing = schedules[groupIndex][slotIndex];
                    var group = groups[groupIndex];


                    var player1IsBye = pairing.Player1Id < 0;
                    var player2IsBye = pairing.Player2Id < 0;

                    var match = new Match
                    {
                        Round = round,
                        Group = group,
                        GroupId = group.Id == 0 ? null : group.Id,
                        BoardId = group.BoardId,

                        Status = (player1IsBye || player2IsBye)
                            ? MatchStatus.Completed
                            : MatchStatus.Scheduled,

                        PlannedStart = plannedStart,
                        PlannedEnd = plannedStart.AddMinutes(matchDurationMinutes)
                    };

                    // Player 1 ist ein echter Spieler
                    if (!player1IsBye)
                    {
                        match.Participants.Add(new MatchParticipant
                        {
                            PlayerId = pairing.Player1Id,
                            IsWinner = player2IsBye
                        });
                    }

                    // Player 2 ist ein echter Spieler
                    if (!player2IsBye)
                    {
                        match.Participants.Add(new MatchParticipant
                        {
                            PlayerId = pairing.Player2Id,
                            IsWinner = player1IsBye
                        });
                    }

                    round.Matches.Add(match);



                    round.PlannedEnd = round.Matches.Count == 0
                        ? plannedStart
                        : round.Matches.Max(m => m.PlannedEnd);
                    rounds.Add(round);
                }
            }

            return rounds;
        }

        private static List<List<Pairing>> CreateRoundRobinSchedule(List<int> playerIds)
        {
            var players = playerIds.ToList();
            if (players.Count % 2 != 0)
                players.Add(-1);

            var result = new List<List<Pairing>>();
            var totalRounds = players.Count - 1;

            for (var round = 0; round < totalRounds; round++)
            {
                var pairings = new List<Pairing>();
                for (var i = 0; i < players.Count / 2; i++)
                {
                    var a = players[i];
                    var b = players[players.Count - 1 - i];
                    if (a != 0 && b != 0)
                        pairings.Add(new Pairing(a, b));
                }

                result.Add(pairings);
                var fixedPlayer = players[0];
                var lastPlayer = players[^1];
                players.RemoveAt(players.Count - 1);
                players.Insert(1, lastPlayer);
                players[0] = fixedPlayer;
            }

            return result;
        }

        private static List<Group> BuildGroups(
            Tournament tournament,
            IReadOnlyList<Player> players,
            GenerateGroupsDto options,
            IReadOnlyList<Board> boards)
        {
            var groups = Enumerable.Range(0, options.GroupCount)
                .Select(i => new Group
                {
                    Tournament = tournament,
                    TournamentId = tournament.Id,
                    Board = boards[i],
                    BoardId = boards[i].Id,
                    Sequence = i + 1,
                    Name = $"Gruppe {i + 1}",
                    QualifiersCount = options.QualifiersPerGroup
                })
                .ToList();

            // Shuffle for fair random distribution, then balance the group sizes as evenly as possible.
            var shuffled = players.OrderBy(_ => Random.Shared.Next()).ToList();
            for (var i = 0; i < shuffled.Count; i++)
            {
                var groupIndex = i % groups.Count;
                groups[groupIndex].Players.Add(new GroupPlayer
                {
                    Player = shuffled[i],
                    PlayerId = shuffled[i].Id,
                    Group = groups[groupIndex]
                });
            }

            //if (groups.Any(g => g.Players.Count < options.QualifiersPerGroup))
            //    throw new InvalidOperationException(
            //        "Die Anzahl der Weiterkommer ist für mindestens eine tatsächlich gebildete Gruppe zu hoch. Verringere die Zahl der Weiterkommer oder passe die Spieler-/Gruppenkonfiguration an.");

            return groups;
        }


        private async Task<List<Player>> LoadPlayersAsync(List<int>? selectedPlayerIds)
        {
            if (selectedPlayerIds == null || selectedPlayerIds.Count == 0)
                return await _context.Players.OrderBy(p => p.Id).ToListAsync();

            var requested = selectedPlayerIds.Distinct().ToList();
            var players = await _context.Players
                .Where(p => requested.Contains(p.Id))
                .OrderBy(p => p.Id)
                .ToListAsync();

            var missing = requested.Except(players.Select(p => p.Id)).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"Folgende Spieler-IDs wurden nicht gefunden: {string.Join(", ", missing)}.");

            return players;
        }

        private async Task<(int locationId, List<Board> boards)?> FindLocationWithBoardsAsync(int requiredBoards)
        {
            var candidates = await _context.Locations
                .Include(l => l.Boards)
                .OrderBy(l => l.Id)
                .ToListAsync();

            foreach (var location in candidates)
            {
                var boards = location.Boards
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.Number)
                    .ToList();
                if (boards.Count >= requiredBoards)
                    return (location.Id, boards);
            }

            return null;
        }

        private static void ValidateTournamentModeForGroupGeneration(Tournament tournament)
        {
            if (tournament.Mode != TournamentMode.GroupStage && tournament.Mode != TournamentMode.GrouStageandKnockout)
                throw new InvalidOperationException("Der Turniermodus wird von der Gruppen-/K.-o.-Generierung nicht unterstützt.");
        }

        private static void ValidateGenerationOptions(GenerateGroupsDto options)
        {
            if (options.GroupCount < 1)
                throw new InvalidOperationException("Die Anzahl der Gruppen muss mindestens 1 sein.");
            if (options.GroupSize < 2)
                throw new InvalidOperationException("Die Gruppengröße muss mindestens 2 Spieler betragen.");
            if (options.QualifiersPerGroup < 1 || options.QualifiersPerGroup > options.GroupSize)
                throw new InvalidOperationException("Die Anzahl der Weiterkommer darf die Gruppengröße nicht überschreiten.");
            if (options.PlayerIds != null && options.PlayerIds.Count == 0)
                throw new InvalidOperationException("Die Spielerliste darf nicht leer sein, wenn PlayerIds übergeben werden.");
        }

        private static void ValidatePlayers(IReadOnlyList<Player> players, int groupCount, int maxGroupSize)
        {
            if (players.Count < groupCount * 2)
                throw new InvalidOperationException(
                    $"Es müssen mindestens {groupCount * 2} Spieler vorhanden sein, damit jede der {groupCount} Gruppen mindestens zwei Spieler enthält.");
            if (players.Count > groupCount * maxGroupSize)
                throw new InvalidOperationException(
                    $"Für {players.Count} Spieler, {groupCount} Gruppen und eine Gruppengröße von maximal {maxGroupSize} fehlen Gruppenkapazitäten.");
        }

        private static void ValidateTiming(int matchDurationMinutes, int breakBetweenMatchesMinutes)
        {
            if (matchDurationMinutes < 1)
                throw new InvalidOperationException("Die Matchdauer muss mindestens 1 Minute betragen.");
            if (breakBetweenMatchesMinutes < 0)
                throw new InvalidOperationException("Die Pause zwischen Matches darf nicht negativ sein.");
        }

        private readonly record struct Pairing(int Player1Id, int Player2Id);
    }
}
