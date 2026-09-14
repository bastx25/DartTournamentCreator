

using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Dtos.Tournament;
using DTC.Api.Models;

namespace DTC.Api.Interfaces
{
    public interface ITournamentConfigRepository
    {
        Task<IEnumerable<TournamentConfig>> GetConfigsAsync(int tournamentid);
        Task UpdateConfig(int tournamentId, GenerateGroupsDto dto);
        Task<TournamentConfig> CreateEmptyConfig(int tournamentId);
        Task <TournamentConfig> CreateNewTConfig(int tournamentId, CreateTournamentDto dto);
    }
}
