using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();
        Task<List<Player>> GetByUserIdAsync(string userId);
        Task<Player?> GetByIdAsync(int id);
        Task<Player?> GetByIdAndUserIdAsync(int id, string userId);
        Task<Player> CreateAsync(Player player);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(int id, string userId);
        Task<Player> UpdateAsync(Player player);
    }
}
