using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class BraketRepository : IBraketRepository
    {
        private readonly DartDbContext _context;
        private readonly IMatchRepository _matchRepo;

        public BraketRepository(DartDbContext context, IMatchRepository matchRepository)
        {
            _context = context;
            _matchRepo = matchRepository;
        }

        public async Task<IEnumerable<Braket>> GetByTournamentIdAsync(int tournamentId)
        {
            return await _context.Brakets
                .Include(r => r.Matches)
                    .ThenInclude(m => m.Participants)
                        .ThenInclude(p => p.TournamentPlayer)
                .Where(r => r.TournamentId == tournamentId)
                .OrderBy(r => r.Sequence)
                .ToListAsync();
        }

        public async Task<Braket?> GetByIdAsync(int id)
        {
            return await _context.Brakets
                .Include(r => r.Matches)
                    .ThenInclude(m => m.Participants)
                        .ThenInclude(p => p.TournamentPlayer)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Braket> CreateAsync(Braket braket)
        {
            await _context.Brakets.AddAsync(braket);
            await _context.SaveChangesAsync();
            return braket;
        }

        public async Task<Braket> UpdateAsync(Braket braket)
        {
            await _context.SaveChangesAsync();
            return braket;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var braket = await _context.Brakets.FindAsync(id);
            if (braket == null) return false;

            _context.Brakets.Remove(braket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Braket>> GetBraketsAsync(int tournamentId)
        {
            return await _context.Brakets.Where(r => r.TournamentId == tournamentId).ToListAsync();
        }

        public async Task DeleteAllTournamentBraketsAsync(int tournamentId)
        {
            var existing = await _context.Brakets.Where(r => r.TournamentId == tournamentId).ToListAsync();

            foreach (var braket in existing)
            {
                await _matchRepo.DeleteByBraketId(braket.Id);
            }

            _context.Brakets.RemoveRange(existing);

            await _context.SaveChangesAsync();
        }
    }
}
