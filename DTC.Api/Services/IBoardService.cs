using DTC.Api.Models;

namespace DTC.Api.Services
{
    public interface IBoardService
    {
        Task<List<Board>> GetActiveBoards();
        Task SetBoards(List<Group> groups, DateTimeOffset startTime, int matchDuration, int breakMinutes);
    }
}
