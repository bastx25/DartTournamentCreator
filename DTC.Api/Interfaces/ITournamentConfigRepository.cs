

using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ITournamentConfigRepository
    {
        Task<IEnumerable<TournamentConfig>> GetConfigsAsync(int tournamentid);
    }
}
