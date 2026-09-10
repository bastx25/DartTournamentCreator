using DTC.Api.Data;
using DTC.Api.Enums;
using DTC.Api.Models;
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

        public async Task SetBoards(List<Group> groups, DateTimeOffset startTime, int matchDuration, int breakMinutes)
        {
            var boards = await GetActiveBoards();

            var matches = GetMatchesFromGroups(groups)
                .Where(m => m.Status != MatchStatus.Completed)
                .OrderBy(m => m.Id)
                .ToList();

            if (!boards.Any())
                throw new InvalidOperationException("Keine aktiven Boards verfügbar.");

            var boardAvailability = boards.ToDictionary(
                board => board.Id,
                board => startTime
            );

            foreach (var match in matches)
            {
                // Board, das am frühesten verfügbar ist
                var availableBoard = boards
                    .OrderBy(board => boardAvailability[board.Id])
                    .First();

                var matchStart = boardAvailability[availableBoard.Id];

                match.BoardId = availableBoard.Id;
                match.PlannedStart = matchStart;
                match.PlannedEnd = matchStart.AddMinutes(matchDuration);

                boardAvailability[availableBoard.Id] =
                    match.PlannedEnd.Value.AddMinutes(breakMinutes);
            }
        }


        public async Task<List<Board>> GetActiveBoards()
        {
            return await _context.Boards.Where(b => b.IsActive).OrderBy(x => x.Id).ToListAsync();
        }

        private List<Match> GetMatchesFromGroups(List<Group> groups)
        {
            var matches = new List<Match>();

            foreach (var group in groups)
            {
                matches.AddRange(group.Matches);
            }

            return matches;

        }
    }
}
