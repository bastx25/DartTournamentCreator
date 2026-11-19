using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DartDbContext _context;

        public PlayerRepository(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
        }

        public async Task<Player> CreateAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var playerModel = await _context.Players.FindAsync(id);
            if (playerModel == null) return false;

            var isInUse = await _context.TournamentPlayers.AnyAsync(tp => tp.PlayerId == id);
            if (isInUse) return false;

            try
            {
                _context.Players.Remove(playerModel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var playerModel = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (playerModel == null) return false;

            var isInUse = await _context.TournamentPlayers.AnyAsync(tp => tp.PlayerId == id);
            if (isInUse) return false;

            try
            {
                _context.Players.Remove(playerModel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Player> UpdateAsync(Player player)
        {
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<List<Player>> GetByUserIdAsync(string userId)
        {
            return await _context.Players
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.LastName ?? p.FirstName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<Player?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }
    }
}
