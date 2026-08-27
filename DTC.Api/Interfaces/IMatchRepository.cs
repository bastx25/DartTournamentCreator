namespace DTC.Api.Interfaces
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetByRoundIdAsync(int roundId);
        Task<Match?> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task<Match> UpdateAsync(Match match);
        Task<bool> DeleteAsync(int id);
    }
}
