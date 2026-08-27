using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface IMatchParticipantRepository
    {
        Task<List<MatchParticipant>> GetByMatchIdAsync(int matchId);
        Task<MatchParticipant?> GetByIdAsync(int id);
        Task<MatchParticipant> CreateAsync(MatchParticipant participant);
        Task<MatchParticipant> UpdateAsync(MatchParticipant participant);
        Task<bool> DeleteAsync(int id);
    }
}
