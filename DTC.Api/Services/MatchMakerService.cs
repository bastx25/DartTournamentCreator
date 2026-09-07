using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Enums;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DTC.Api.Services
{
    public class MatchMakerService : IMatchMakerService
    {
        private readonly DartDbContext _context;

        public MatchMakerService(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
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