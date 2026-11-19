using DTC.Api.Data;
using DTC.Api.Models;
using DTC.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DTC.Api.Tests
{
    public class TournamentAuthorizationTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        [Fact]
        public async Task HasTournamentAccess_ReturnsTrueForOwner_FalseForNonOwner()
        {
            using var context = CreateInMemoryContext("AuthTest_" + Guid.NewGuid());
            var authService = new TournamentAuthorizationService(context);

            var ownerId = "user-123";
            var attackerId = "user-999";

            var tournament = new Tournament
            {
                Id = 1,
                Name = "Owner Tournament",
                Code = "DART-OWN123",
                UserId = ownerId,
                StartDate = DateTimeOffset.UtcNow
            };
            context.Tournaments.Add(tournament);
            await context.SaveChangesAsync();

            Assert.True(await authService.HasTournamentAccessAsync(1, ownerId));
            Assert.False(await authService.HasTournamentAccessAsync(1, attackerId));
            Assert.False(await authService.HasTournamentAccessAsync(1, ""));
            Assert.False(await authService.HasTournamentAccessAsync(999, ownerId));
        }

        [Fact]
        public async Task SubObjectAccess_ProtectedAgainstIDOR()
        {
            using var context = CreateInMemoryContext("SubObjectAuthTest_" + Guid.NewGuid());
            var authService = new TournamentAuthorizationService(context);

            var ownerId = "owner-1";
            var attackerId = "attacker-2";

            var tournament = new Tournament
            {
                Id = 10,
                Name = "Protected Tournament",
                Code = "DART-SUB123",
                UserId = ownerId,
                StartDate = DateTimeOffset.UtcNow
            };
            context.Tournaments.Add(tournament);

            var group = new Group
            {
                Id = 100,
                TournamentId = 10,
                Tournament = tournament,
                Name = "Gruppe A"
            };
            context.Groups.Add(group);

            var braket = new Braket
            {
                Id = 200,
                TournamentId = 10,
                Tournament = tournament,
                Name = "Finale"
            };
            context.Brakets.Add(braket);

            var player = new Player { Id = 5, FirstName = "Max", LastName = "Mustermann" };
            context.Players.Add(player);

            var tp = new TournamentPlayer
            {
                Id = 300,
                TournamentId = 10,
                Tournament = tournament,
                PlayerId = 5,
                Player = player
            };
            context.TournamentPlayers.Add(tp);

            var groupMatch = new Match
            {
                Id = 400,
                GroupId = 100,
                Group = group
            };
            context.Matches.Add(groupMatch);

            await context.SaveChangesAsync();

            // Owner checks
            Assert.True(await authService.HasGroupAccessAsync(100, ownerId));
            Assert.True(await authService.HasBraketAccessAsync(200, ownerId));
            Assert.True(await authService.HasTournamentPlayerAccessAsync(300, ownerId));
            Assert.True(await authService.HasMatchAccessAsync(400, ownerId));

            // Attacker checks (MUST ALL FAIL)
            Assert.False(await authService.HasGroupAccessAsync(100, attackerId));
            Assert.False(await authService.HasBraketAccessAsync(200, attackerId));
            Assert.False(await authService.HasTournamentPlayerAccessAsync(300, attackerId));
            Assert.False(await authService.HasMatchAccessAsync(400, attackerId));
        }
    }
}
