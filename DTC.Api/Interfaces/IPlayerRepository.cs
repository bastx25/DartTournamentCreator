using DTC.Api.Dtos.Player;
using DTC.Api.Models;
using System.Numerics;

namespace DTC.Api.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();
        Task<Player> GetByIdAsync(int id);
        Task<Player?> CreateAsync(Player player);
        Task<bool> DeleteAsync(int id);
        Task<Player> UpdateAsync(Player player);
    }
}
