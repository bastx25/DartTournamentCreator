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

        Task<bool> HasPlayerAccessAsync(int playerId, string userId);
        Task<bool> HasAllPlayersAccessAsync(IEnumerable<int> playerIds, string userId);
        Task<bool> HasLocationAccessAsync(int locationId, string userId);
        Task<bool> HasBoardAccessAsync(int boardId, string userId);
        Task<int> GetActiveBoardsCountAsync(string userId);
        Task<int> GetLocationsCountAsync(string userId);
    }
}
