using DTC.Api.Data;
using DTC.Api.Dtos.Player;
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

            _context.Players.Remove(playerModel);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<Player> UpdateAsync(Player player)
        {
            // Die Entity ist bereits vom DbContext gecached/getrackt,
            // Change Tracking erkennt geänderte Properties automatisch.

            await _context.SaveChangesAsync();

            return player;
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

    }
}
