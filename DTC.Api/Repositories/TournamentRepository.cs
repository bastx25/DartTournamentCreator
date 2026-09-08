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
            return await _context.Tournaments.ToListAsync();
        }


        public async Task<Tournament?> GetByIdAsync(int id)
        {
            return await _context.Tournaments

                // Tournament → Rounds → Matches → Participants → Player
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.Participants)

                // Tournament → Groups → GroupPlayers → Player
                .Include(t => t.Groups)
                    .ThenInclude(g => g.GroupPlayers)


                // Tournament → Groups → Rounds → Matches → Participants → Player
                .Include(t => t.Groups)
                        .ThenInclude(r => r.Matches)
                            .ThenInclude(m => m.Participants)

                .Include(t => t.TournamentPlayers)
                    .ThenInclude(tp => tp.Player)


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
