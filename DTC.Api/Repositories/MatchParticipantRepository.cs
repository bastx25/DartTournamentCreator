using DTC.Api.Data;
using DTC.Api.Interfaces;

namespace DTC.Api.Repositories
{
    public class MatchParticipantRepository : IMatchParticipantRepository
    {
        private readonly DartDbContext _context;

        public MatchParticipantRepository(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
        }
    }
}
