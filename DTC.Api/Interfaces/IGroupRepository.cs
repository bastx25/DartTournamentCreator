

using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetGroupsByTournamentId(int id);
    }
}
