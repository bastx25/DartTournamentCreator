using DTC.Api.Interfaces;

namespace DTC.Api.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        public Task<Match> CreateAsync(Match match)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Match?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Match>> GetByRoundIdAsync(int roundId)
        {
            throw new NotImplementedException();
        }

        public Task<Match> UpdateAsync(Match match)
        {
            throw new NotImplementedException();
        }
    }
}
