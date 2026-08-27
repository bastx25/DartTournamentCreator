using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class MatchParticipantRepository : IMatchParticipantRepository
    {
        private readonly DartDbContext _context;

        public MatchParticipantRepository(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
        }

        public async Task<IEnumerable<MatchParticipant>> GetByMatchIdAsync(int matchId)
        {
            return await _context.MatchParticipants
                .Include(mp => mp.Player)
                .Where(mp => mp.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<MatchParticipant?> GetByIdAsync(int id)
        {
            return await _context.MatchParticipants
                .Include(mp => mp.Player)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }

        public async Task<MatchParticipant> CreateAsync(MatchParticipant participant)
        {
            await _context.MatchParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();

            // Player nachladen, damit der neu erstelle Record mit Player-Data fürs DTO bereitsteht
            await _context.Entry(participant).Reference(p => p.Player).LoadAsync();

            return participant;
        }

        public async Task<MatchParticipant> UpdateAsync(MatchParticipant participant)
        {
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var participant = await _context.MatchParticipants.FindAsync(id);
            if (participant == null) return false;

            _context.MatchParticipants.Remove(participant);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
