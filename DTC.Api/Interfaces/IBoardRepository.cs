using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IBoardRepository
    {
        Task<IEnumerable<Board>> GetByLocationIdAsync(int locationId);
        Task<Board?> GetByIdAsync(int id);
        Task<Board> CreateAsync(Board board);
        Task<Board> UpdateAsync(Board board);
        Task<bool> DeleteAsync(int id);
    }
}
