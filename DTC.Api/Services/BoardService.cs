using DTC.Api.Data;
using DTC.Api.Enums;
using DTC.Api.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Services
{
    public class BoardService : IBoardService
    {
        private readonly DartDbContext _context;

        public BoardService(DartDbContext context)
        {
            _context = context;
        }

        public async Task SetBoards(
    List<Group> groups,
    DateTimeOffset startTime,
    int matchDuration,
    int breakMinutes)
        {
            var boards = await GetActiveBoards();

            var matches = GetMatchesFromGroups(groups)
                .Where(m => m.Status != MatchStatus.Completed)
                .OrderBy(m => m.Id)
                .ToList();

            if (!boards.Any())
                throw new InvalidOperationException("Keine aktiven Boards verfügbar.");

            // Alle TournamentPlayer laden, die in den zu planenden Matches vorkommen.
            var tournamentPlayerIds = matches
                .SelectMany(m => m.Participants)
                .Select(mp => mp.TournamentPlayerId)
                .Distinct()
                .ToList();

            // Wichtig:
            // Availabilities explizit laden, weil die Collection beim Aufruf
            // möglicherweise noch nicht geladen wurde.
            var availabilities = await _context.Availabilities
                .Where(a => tournamentPlayerIds.Contains(a.TournamentPlayerId))
                .ToListAsync();

            var playerAvailabilities = availabilities
                .GroupBy(a => a.TournamentPlayerId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(a => a.Start).ToList());

            // Slots, die durch bereits geplante Matches in dieser Planung
            // für einen TournamentPlayer belegt sind.
            var playerBusySlots =
                new Dictionary<int, List<(DateTimeOffset Start, DateTimeOffset End)>>();

            foreach (var playerId in tournamentPlayerIds)
            {
                playerBusySlots[playerId] =
                    new List<(DateTimeOffset Start, DateTimeOffset End)>();
            }

            // Wann ist welches Board wieder verfügbar?
            var boardAvailability = boards.ToDictionary(
                board => board.Id,
                board => startTime);

            foreach (var match in matches)
            {
                var participants = match.Participants
                    .ToList();

                if (participants.Count != 2)
                {
                    throw new InvalidOperationException(
                        $"Match {match.Id} hat nicht genau 2 MatchParticipants.");
                }

                var player1Id = participants[0].TournamentPlayerId;
                var player2Id = participants[1].TournamentPlayerId;

                if (!playerAvailabilities.ContainsKey(player1Id))
                {
                    throw new InvalidOperationException(
                        $"TournamentPlayer {player1Id} hat keine Availability.");
                }

                if (!playerAvailabilities.ContainsKey(player2Id))
                {
                    throw new InvalidOperationException(
                        $"TournamentPlayer {player2Id} hat keine Availability.");
                }

                DateTimeOffset? selectedStart = null;
                DateTimeOffset? selectedEnd = null;
                Board? selectedBoard = null;

                // Boards nach frühester Verfügbarkeit durchsuchen.
                foreach (var board in boards
                    .OrderBy(b => boardAvailability[b.Id]))
                {
                    var earliestPossibleStart = boardAvailability[board.Id];

                    var playerSlot = FindEarliestCommonFreeSlot(
                        earliestPossibleStart,
                        matchDuration,
                        playerAvailabilities[player1Id],
                        playerAvailabilities[player2Id],
                        playerBusySlots[player1Id],
                        playerBusySlots[player2Id]);

                    if (playerSlot == null)
                        continue;

                    selectedStart = playerSlot.Value.Start;
                    selectedEnd = playerSlot.Value.End;
                    selectedBoard = board;

                    break;
                }

                if (selectedBoard == null ||
                    selectedStart == null ||
                    selectedEnd == null)
                {
                    throw new InvalidOperationException(
                        $"Für Match {match.Id} konnte kein gemeinsamer " +
                        $"verfügbarer Zeitraum für beide Spieler gefunden werden.");
                }

                // Match planen
                match.BoardId = selectedBoard.Id;
                match.PlannedStart = selectedStart;
                match.PlannedEnd = selectedEnd;

                // Board für nächstes Match blockieren
                boardAvailability[selectedBoard.Id] =
                    selectedEnd.Value.AddMinutes(breakMinutes);

                // Beide Spieler inklusive Pause für weitere Matches blockieren
                var endWithBreak = selectedEnd.Value.AddMinutes(breakMinutes);

                // Beide Spieler für diesen Zeitraum blockieren
                playerBusySlots[player1Id].Add(
                    (selectedStart.Value, endWithBreak));

                playerBusySlots[player2Id].Add(
                    (selectedStart.Value, endWithBreak));
            }
        }

        private static (DateTimeOffset Start, DateTimeOffset End)?
    FindEarliestCommonFreeSlot(
        DateTimeOffset earliestStart,
        int durationMinutes,
        List<Availability> player1Availabilities,
        List<Availability> player2Availabilities,
        List<(DateTimeOffset Start, DateTimeOffset End)> player1BusySlots,
        List<(DateTimeOffset Start, DateTimeOffset End)> player2BusySlots)
        {
            var duration = TimeSpan.FromMinutes(durationMinutes);

            foreach (var availability1 in player1Availabilities)
            {
                foreach (var availability2 in player2Availabilities)
                {
                    // Schnittmenge der beiden Availability-Fenster
                    var commonStart = Max(
                        availability1.Start,
                        availability2.Start,
                        earliestStart);

                    var commonEnd = Min(
                        availability1.End,
                        availability2.End);

                    if (commonStart + duration > commonEnd)
                        continue;

                    var candidateStart = commonStart;

                    while (candidateStart + duration <= commonEnd)
                    {
                        var candidateEnd = candidateStart + duration;

                        var player1Conflict = player1BusySlots.Any(
                            busy => Overlaps(
                                candidateStart,
                                candidateEnd,
                                busy.Start,
                                busy.End));

                        var player2Conflict = player2BusySlots.Any(
                            busy => Overlaps(
                                candidateStart,
                                candidateEnd,
                                busy.Start,
                                busy.End));

                        if (!player1Conflict && !player2Conflict)
                        {
                            return (candidateStart, candidateEnd);
                        }

                        // Wir müssen nach dem nächsten belegten Slot weitersuchen.
                        var nextBusyStart = player1BusySlots
                            .Concat(player2BusySlots)
                            .Where(b => b.End > candidateStart)
                            .Select(b => b.Start)
                            .OrderBy(x => x)
                            .FirstOrDefault();

                        if (nextBusyStart == default)
                            break;

                        candidateStart = Max(
                            candidateStart.AddMinutes(1),
                            nextBusyStart);
                    }
                }
            }

            return null;
        }

        private static bool Overlaps(
    DateTimeOffset start1,
    DateTimeOffset end1,
    DateTimeOffset start2,
    DateTimeOffset end2)
        {
            return start1 < end2 && end1 > start2;
        }

        private static DateTimeOffset Max(
            params DateTimeOffset[] values)
        {
            return values.Max();
        }

        private static DateTimeOffset Min(
            params DateTimeOffset[] values)
        {
            return values.Min();
        }


        public async Task<List<Board>> GetActiveBoards()
        {
            return await _context.Boards.Where(b => b.IsActive).OrderBy(x => x.Id).ToListAsync();
        }

        public async Task SetBoards(
    List<Braket> brakets,
    DateTimeOffset startTime,
    int matchDuration,
    int breakMinutes)
        {
            var boards = await GetActiveBoards();

            if (!boards.Any())
                throw new InvalidOperationException("Keine aktiven Boards verfügbar.");

            foreach(var braket in brakets)
            {
                var matches = braket.Matches
                    .Where(m => m.Status == MatchStatus.Completed)
                    .ToList()


            }
        }

        private List<Match> GetMatchesFromGroups(List<Group> groups)
        {
            var matches = new List<Match>();

            int maxMatches = groups.Max(g => g.Matches.Count);

            for (int i = 0; i < maxMatches; i++)
            {
                foreach (var group in groups)
                {
                    if (i < group.Matches.Count)
                    {
                        matches.Add(group.Matches.ElementAt(i));
                    }
                }
            }

            return matches;
        }

        private List<Match> GetMatchesFromBrakets(List<Braket> brakets)
        {
            var matches = new List<Match>();

            foreach (var braket in brakets)
            {
                matches.AddRange(braket.Matches);
            }

            return matches;

        }
    }
}
