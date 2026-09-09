using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class RoundRepository : IRoundRepository
    {
        private readonly DartDbContext _context;
        private readonly IMatchRepository _matchRepo;

        public RoundRepository(DartDbContext context, IMatchRepository matchRepository)
        {
            _context = context;
            _matchRepo = matchRepository;
        }

        public async Task<IEnumerable<Round>> GetByTournamentIdAsync(int tournamentId)
        {
            return await _context.Rounds
                .Include(r => r.Matches)
                    .ThenInclude(m => m.Participants)
                        .ThenInclude(p => p.TournamentPlayer)
                .Where(r => r.TournamentId == tournamentId)
                .OrderBy(r => r.Sequence)
                .ToListAsync();
        }

        public async Task<Round?> GetByIdAsync(int id)
        {
            return await _context.Rounds
                .Include(r => r.Matches)
                    .ThenInclude(m => m.Participants)
                        .ThenInclude(p => p.TournamentPlayer)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Round> CreateAsync(Round round)
        {
            await _context.Rounds.AddAsync(round);
            await _context.SaveChangesAsync();
            return round;
        }

        public async Task<Round> UpdateAsync(Round round)
        {
            await _context.SaveChangesAsync();
            return round;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var round = await _context.Rounds.FindAsync(id);
            if (round == null) return false;

            _context.Rounds.Remove(round);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Round>> GetRoundsByTournamentId(int tournamentId)
        {
            return await _context.Rounds.Where(r => r.TournamentId == tournamentId).ToListAsync();
        }

        public async Task DeleteAllTournamentRoundsAsync(int tournamentId)
        {
            var existing = await _context.Rounds.Where(r => r.TournamentId == tournamentId).ToListAsync();

            foreach (var round in existing)
            {
                await _matchRepo.DeleteByRoundId(round.Id);
            }

            _context.Rounds.RemoveRange(existing);

            await _context.SaveChangesAsync();
        }
    }
}
