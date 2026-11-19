using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ITournamentAuthorizationService
    {
        Task<Tournament?> GetAuthorizedTournamentAsync(int tournamentId, string userId);
        Task<bool> HasTournamentAccessAsync(int tournamentId, string userId);
        Task<bool> HasGroupAccessAsync(int groupId, string userId);
        Task<bool> HasBraketAccessAsync(int braketId, string userId);
        Task<bool> HasMatchAccessAsync(int matchId, string userId);
        Task<bool> HasTournamentPlayerAccessAsync(int tournamentPlayerId, string userId);
    }
}
