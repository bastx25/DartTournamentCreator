using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Models;

namespace DTC.Api.Services
{
    public interface IMatchMakerService
    {
        Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options);
        Task GenerateKnockoutAsync(int tournamentId, GenerateGroupsDto options);
        Task AdvanceKnockoutAsync(int matchId);
        Task AdvanceWinner(Match match);
    }
}
