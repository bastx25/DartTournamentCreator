using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private DartDbContext _context;
        private readonly IMatchRepository _matchRepo;

        public GroupRepository(DartDbContext context, IMatchRepository matchRepository)
        {
            _context = context;
            _matchRepo = matchRepository;
        }

        public async Task DeleteAllTournamentGroups(int tournamentId)
        {
            var existing = await _context.Groups.Where(g => g.TournamentId == tournamentId).ToListAsync();

            foreach (var group in existing) 
            {
               await _matchRepo.DeleteByGroupId(group.Id);
            }

            _context.Groups.RemoveRange(existing);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync(int tournamentId)
        {
            return await _context.Groups.Where(g => g.TournamentId == tournamentId).OrderBy(g => g.Name).ToListAsync();
        }

        public async Task DeleteAllWithNameAsync(string name)
        {
            var existing = await _context.Groups.Where(g => g.Name == "Tiebreaker").ToListAsync();

            _context.Groups.RemoveRange(existing);

            await _context.SaveChangesAsync();
        }
    }
}
