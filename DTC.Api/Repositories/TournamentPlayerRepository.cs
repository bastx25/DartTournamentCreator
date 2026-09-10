using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class TournamentPlayerRepository : ITournamentPlayerRepository
    {
        private readonly DartDbContext _context;

        public TournamentPlayerRepository(DartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TournamentPlayer>> GetPlayersAsync(int tournamentid)
        {
            return await _context.TournamentPlayers.Where(x => x.TournamentId == tournamentid).ToListAsync();
        }

        public async Task SetTournamentPlayers(int tournamentId, List<int>? playerIds)
        {
            var existingPlayers = await _context.TournamentPlayers
                .Where(p => p.TournamentId == tournamentId)
                .ToListAsync();

            _context.TournamentPlayers.RemoveRange(existingPlayers);
            await _context.SaveChangesAsync();

            if(playerIds == null) playerIds = new List<int>();

            var newPlayers = await _context.Players
                .Where(p => playerIds.Contains(p.Id))
                .ToListAsync();

            foreach (var player in newPlayers)
            {
                await _context.TournamentPlayers.AddAsync(new TournamentPlayer
                {
                    TournamentId = tournamentId,
                    PlayerId = player.Id
                });
            }

            await _context.SaveChangesAsync();
        }

    }
}
