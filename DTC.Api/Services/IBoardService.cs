using DTC.Api.Models;

namespace DTC.Api.Services
{
    public interface IBoardService
    {
        Task SetBoards(List<Group> groups, DateTimeOffset startTime, int matchDuration, int breakMinutes, string? userId = null);
        Task SetBoards(List<Braket> brakets, DateTimeOffset startTime, int matchDuration, int breakMinutes, string? userId = null);
        Task<List<Board>> GetActiveBoards(string? userId = null);
    }
}
