using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IBraketRepository
    {
        Task<IEnumerable<Braket>> GetByTournamentIdAsync(int tournamentId);
        Task<Braket?> GetByIdAsync(int id);
        Task<Braket> CreateAsync(Braket round);
        Task<Braket> UpdateAsync(Braket round);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Braket>> GetBraketsAsync(int id);
        Task DeleteAllTournamentBraketsAsync(int id);
    }
}
