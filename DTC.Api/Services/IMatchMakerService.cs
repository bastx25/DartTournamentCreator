using DTC.Api.Dtos.MatchMaker;

namespace DTC.Api.Services
{
    public interface IMatchMakerService
    {
        Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options);
        Task GenerateKnockoutAsync(int tournamentId);
    }
}
