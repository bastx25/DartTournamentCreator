

using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IGroupRepository
    {
        Task DeleteAllTournamentGroups(int id);
        Task<IEnumerable<Group>> GetGroupsByTournamentId(int id);
    }
}
