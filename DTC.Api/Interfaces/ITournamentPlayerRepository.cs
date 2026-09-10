using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ITournamentPlayerRepository
    {
        Task<IEnumerable<TournamentPlayer>> GetPlayersAsync(int tournamentid);
        Task SetTournamentPlayers(int id, List<int>? playerIds);
    }
}
