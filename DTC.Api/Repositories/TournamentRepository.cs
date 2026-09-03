using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly DartDbContext _context;

        public TournamentRepository(DartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            return await _context.Tournaments
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.Participants)
                            .ThenInclude(p => p.Player)
                .ToListAsync();
        }

        public async Task<Tournament?> GetByIdAsync(int id)
        {
            return await _context.Tournaments
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.Participants)
                            .ThenInclude(p => p.Player)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tournament> CreateAsync(Tournament tournament)
        {
            await _context.Tournaments.AddAsync(tournament);
            await _context.SaveChangesAsync();
            return tournament;
        }

        public async Task<Tournament> UpdateAsync(Tournament tournament)
        {
            await _context.SaveChangesAsync();
            return tournament;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return false;

            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
