using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetByBraketIdAsync(int roundId);
        Task<Match?> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task<Match> UpdateAsync(Match match);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Match>> GetByGroupIdAsync(int groupId);
        Task DeleteByGroupId(int id);
        Task DeleteByBraketId(int id);
    }
}
