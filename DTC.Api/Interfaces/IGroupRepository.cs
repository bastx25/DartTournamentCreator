

using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IGroupRepository
    {
        Task DeleteAllWithNameAsync(string name);
        Task DeleteAllTournamentGroups(int id);
        Task<IEnumerable<Group>> GetGroupsAsync(int id);
    }
}
