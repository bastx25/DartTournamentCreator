using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly DartDbContext _context;

        public BoardRepository(DartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Board>> GetByLocationIdAsync(int locationId)
        {
            return await _context.Boards
                .Where(b => b.LocationId == locationId)
                .OrderBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Board>> GetByLocationIdAndUserIdAsync(int locationId, string userId)
        {
            return await _context.Boards
                .Where(b => b.LocationId == locationId && b.UserId == userId)
                .OrderBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<Board?> GetByIdAsync(int id)
        {
            return await _context.Boards.FindAsync(id);
        }

        public async Task<Board?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Boards
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }

        public async Task<Board> CreateAsync(Board board)
        {
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();
            return board;
        }

        public async Task<Board> UpdateAsync(Board board)
        {
            await _context.SaveChangesAsync();
            return board;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var board = await _context.Boards.FindAsync(id);
            if (board == null) return false;

            var matches = await _context.Matches
                .Where(m => m.BoardId == id)
                .ToListAsync();
            foreach (var match in matches)
            {
                match.BoardId = null;
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var board = await _context.Boards
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (board == null) return false;

            var matches = await _context.Matches
                .Where(m => m.BoardId == id)
                .ToListAsync();
            foreach (var match in matches)
            {
                match.BoardId = null;
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Board>> GetAllAsync()
        {
            return await _context.Boards
                .OrderBy(b => b.LocationId)
                .ThenBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Board>> GetByUserIdAsync(string userId)
        {
            return await _context.Boards
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.LocationId)
                .ThenBy(b => b.Id)
                .ToListAsync();
        }
    }
}
