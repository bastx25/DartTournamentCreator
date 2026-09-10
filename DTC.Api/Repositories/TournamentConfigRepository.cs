using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
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

        public async Task UpdateConfig(int id, GenerateGroupsDto dto)
        {
            var config = await _context.TournamentConfigs.FirstOrDefaultAsync(x => x.TournamentId == id);

            if (config == null) return;

            config.GroupCount = dto.GroupCount;
            config.PlayersPerGroup = dto.PlayersPerGroup;
            config.QualifiersPerGroup = dto.QualifiersPerGroup;
            config.MatchDurationMinutes = dto.MatchDurationMinutes.Value;
            config.BreakBetweenMatchesMinutes = dto.BreakBetweenMatchesMinutes.Value;

            await _context.SaveChangesAsync();
        }
    }
}
