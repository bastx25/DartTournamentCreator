using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Enums;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Services
{
    public class MatchMakerService : IMatchMakerService
    {
        private const int ByePlayerId = -1;

        private readonly DartDbContext _context;

        public MatchMakerService(DartDbContext context)
        {
            _context = context;
        }

        public Task GenerateGroupsAsync(int tournamentId, GenerateGroupsDto options)
        {
            throw new NotImplementedException();
        }

        public Task GenerateKnockoutAsync(int tournamentId)
        {
            throw new NotImplementedException();
        }
    }
}
