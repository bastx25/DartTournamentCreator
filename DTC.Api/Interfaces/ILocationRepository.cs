using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task<Location> CreateAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int id);
    }
}
