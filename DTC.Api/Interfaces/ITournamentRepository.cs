using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ITournamentRepository
    {
        Task<IEnumerable<Tournament>> GetAllAsync();
        Task<IEnumerable<Tournament>> GetByUserIdAsync(string userId);
        Task<Tournament?> GetByIdAsync(int id);
        Task<Tournament?> GetByCodeAsync(string code);
        Task<Tournament> CreateAsync(Tournament tournament);
        Task<Tournament> UpdateAsync(Tournament tournament);
        Task<bool> DeleteAsync(int id);
    }
}
