using DTC.Api.Data;
using DTC.Api.Enums;
using DTC.Api.Models;
using DTC.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DTC.Api.Tests
{
    public class PublicTournamentServiceTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        [Fact]
        public async Task GetTournamentByCode_ReturnsPublicTournamentDto_WhenExists()
        {
            using var context = CreateInMemoryContext("PublicCodeTest_" + Guid.NewGuid());
            var service = new PublicTournamentService(context);

            var tournament = new Tournament
            {
                Id = 1,
                Name = "Public Open 2026",
                Code = "DART-PUB123",
                Status = TournamentStatus.InProgress,
                StartDate = DateTimeOffset.UtcNow,
                UserId = "user-1"
            };
            context.Tournaments.Add(tournament);

            var group = new Group
            {
                Id = 10,
                TournamentId = 1,
                Name = "Gruppe A",
                Sequence = 1
            };
            context.Groups.Add(group);

            await context.SaveChangesAsync();

            var result = await service.GetTournamentByCodeAsync("dart-pub123");

            Assert.NotNull(result);
            Assert.Equal("Public Open 2026", result.Name);
            Assert.Equal("DART-PUB123", result.Code);
            Assert.Single(result.Groups);
            Assert.Equal("Gruppe A", result.Groups[0].Name);
        }

        [Fact]
        public async Task GetTournamentByCode_ReturnsNull_WhenNotExists()
        {
            using var context = CreateInMemoryContext("PublicCodeNullTest_" + Guid.NewGuid());
            var service = new PublicTournamentService(context);

            var result = await service.GetTournamentByCodeAsync("DART-NONEXIST");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTournamentById_ReturnsPublicTournamentDto_WhenExists()
        {
            using var context = CreateInMemoryContext("PublicIdTest_" + Guid.NewGuid());
            var service = new PublicTournamentService(context);

            var tournament = new Tournament
            {
                Id = 42,
                Name = "Dart Masters",
                Code = "DART-MST420",
                Status = TournamentStatus.InProgress,
                StartDate = DateTimeOffset.UtcNow
            };
            context.Tournaments.Add(tournament);
            await context.SaveChangesAsync();

            var result = await service.GetTournamentByIdAsync(42);

            Assert.NotNull(result);
            Assert.Equal(42, result.Id);
            Assert.Equal("DART-MST420", result.Code);
        }

        [Fact]
        public async Task GetActivePublicTournaments_ReturnsOnlyActiveTournaments()
        {
            using var context = CreateInMemoryContext("ActivePubTournamentsTest_" + Guid.NewGuid());
            var service = new PublicTournamentService(context);

            context.Tournaments.AddRange(
                new Tournament
                {
                    Id = 1,
                    Name = "Draft Tournament",
                    Code = "DART-DRF001",
                    Status = TournamentStatus.Draft,
                    StartDate = DateTimeOffset.UtcNow
                },
                new Tournament
                {
                    Id = 2,
                    Name = "Running Tournament",
                    Code = "DART-RUN002",
                    Status = TournamentStatus.InProgress,
                    StartDate = DateTimeOffset.UtcNow
                },
                new Tournament
                {
                    Id = 3,
                    Name = "Completed Tournament",
                    Code = "DART-CMP003",
                    Status = TournamentStatus.Completed,
                    StartDate = DateTimeOffset.UtcNow.AddDays(-5)
                }
            );
            await context.SaveChangesAsync();

            var result = (await service.GetActivePublicTournamentsAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, t => t.Id == 3);
            Assert.Contains(result, t => t.Id == 1);
            Assert.Contains(result, t => t.Id == 2);
        }
    }
}
