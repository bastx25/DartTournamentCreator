using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly DartDbContext _context;

        public MatchRepository(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
        }

        public async Task<IEnumerable<Match>> GetByRoundIdAsync(int roundId)
        {
            return await _context.Matches
                .Include(m => m.Participants)
                    .ThenInclude(p => p.Player)
                .Where(m => m.RoundId == roundId)
                .ToListAsync();
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.Participants)
                    .ThenInclude(p => p.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Match> CreateAsync(Match match)
        {
            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> UpdateAsync(Match match)
        {
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
