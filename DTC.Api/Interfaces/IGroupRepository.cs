

using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IGroupRepository
    {
        Task DeleteAllTiebreakerAsync();
        Task DeleteAllTournamentGroups(int id);
        Task<IEnumerable<Group>> GetGroupsByTournamentIdAsync(int id);
    }
}
