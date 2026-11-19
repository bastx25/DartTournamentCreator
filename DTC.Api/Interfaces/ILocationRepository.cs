using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<IEnumerable<Location>> GetByUserIdAsync(string userId);
        Task<Location?> GetByIdAsync(int id);
        Task<Location?> GetByIdAndUserIdAsync(int id, string userId);
        Task<Location> CreateAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
