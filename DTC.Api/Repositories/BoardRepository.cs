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
                .ToListAsync();
        }

        public async Task<Board?> GetByIdAsync(int id)
        {
            return await _context.Boards.FindAsync(id);
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

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
