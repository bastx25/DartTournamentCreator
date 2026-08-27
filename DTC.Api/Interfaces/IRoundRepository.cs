using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IRoundRepository
    {
        Task<IEnumerable<Round>> GetByTournamentIdAsync(int tournamentId);
        Task<Round?> GetByIdAsync(int id);
        Task<Round> CreateAsync(Round round);
        Task<Round> UpdateAsync(Round round);
        Task<bool> DeleteAsync(int id);
    }
}
