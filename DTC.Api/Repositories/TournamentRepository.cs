using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly DartDbContext _context;
        private readonly IGroupRepository _groupRepo;
        private readonly IBraketRepository _braketRepo;

        public TournamentRepository(DartDbContext context, IGroupRepository groupRepository, IBraketRepository braketRepository)
        {
            _context = context;
            _groupRepo = groupRepository;
            _braketRepo = braketRepository;
        }

        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            return await _context.Tournaments.ToListAsync();
        }


        public async Task<Tournament?> GetByIdAsync(int id)
        {
            return await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);
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

            await _groupRepo.DeleteAllTournamentGroups(id);
            await _braketRepo.DeleteAllTournamentBraketsAsync(id);

            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
