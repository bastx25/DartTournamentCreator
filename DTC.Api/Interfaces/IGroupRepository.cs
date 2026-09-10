

using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IGroupRepository
    {
        Task DeleteAllWithNameAsync(string name);
        Task DeleteAllTournamentGroups(int id);
        Task<List<Group>> GetGroupsAsync(int id);
    }
}
