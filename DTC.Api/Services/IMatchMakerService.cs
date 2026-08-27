namespace DTC.Api.Services
{
    public interface IMatchMakerService
    {
        Task<bool> GenerateRandomGroupsAsync(int tournamentId, int groupSize, List<int>? selectedPlayerIds = null);
    }
}
