using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly DartDbContext _dartDbContext;

        public PlayerRepository(DartDbContext dartDbContext)
        {
            _dartDbContext = dartDbContext;
        }
        public async Task<List<Player>> GetAllAsync()
        {
            return await _dartDbContext.Players.ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _dartDbContext.Players.FindAsync(id);
        }
    }
}
