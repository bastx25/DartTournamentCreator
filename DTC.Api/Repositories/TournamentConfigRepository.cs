using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class TournamentConfigRepository : ITournamentConfigRepository
    {
        private readonly DartDbContext _context;

        public TournamentConfigRepository(DartDbContext context)
        {
            _context = context;   
        }
        public async Task<IEnumerable<TournamentConfig>> GetConfigsAsync(int tournamentid)
        {
            return await _context.TournamentConfigs.Where(c => c.TournamentId == tournamentid).OrderBy(x => x.VersionNr).ToListAsync();
        }
    }
}
