using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private DartDbContext _context;

        public GroupRepository(DartDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAllTournamentGroups(int tournamentId)
        {
            var existing = await _context.Groups.Where(g => g.TournamentId == tournamentId).ToListAsync();

            _context.Groups.RemoveRange(existing);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Group>> GetGroupsByTournamentId(int tournamentId)
        {
            return await _context.Groups.Where(g => g.TournamentId == tournamentId).ToListAsync();
        }
    }
}
