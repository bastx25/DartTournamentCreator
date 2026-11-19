using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IBoardRepository
    {
        Task<IEnumerable<Board>> GetAllAsync();
        Task<IEnumerable<Board>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Board>> GetByLocationIdAsync(int locationId);
        Task<IEnumerable<Board>> GetByLocationIdAndUserIdAsync(int locationId, string userId);
        Task<Board?> GetByIdAsync(int id);
        Task<Board?> GetByIdAndUserIdAsync(int id, string userId);
        Task<Board> CreateAsync(Board board);
        Task<Board> UpdateAsync(Board board);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
