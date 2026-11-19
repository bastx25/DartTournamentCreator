using DTC.Api.Data;
using DTC.Api.Enums;
using DTC.Api.Models;
using DTC.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DTC.Api.Tests
{
    public class CascadeDeleteTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        [Fact]
        public async Task DeleteLocation_CleansUpBoards_AndNullifiesMatches()
        {
            using var context = CreateInMemoryContext("DeleteLocation_" + Guid.NewGuid());
            var locRepo = new LocationRepository(context);

            var loc = new Location { Id = 1, Name = "Vereinsheim", UserId = "user-1" };
            context.Locations.Add(loc);

            var board1 = new Board { Id = 10, LocationId = 1, Label = "Board 1", UserId = "user-1", IsActive = true };
            var board2 = new Board { Id = 11, LocationId = 1, Label = "Board 2", UserId = "user-1", IsActive = true };
            context.Boards.AddRange(board1, board2);

            var match1 = new Match { Id = 100, BoardId = 10, Status = MatchStatus.Scheduled };
            var match2 = new Match { Id = 101, BoardId = 11, Status = MatchStatus.Scheduled };
            var match3 = new Match { Id = 102, BoardId = null, Status = MatchStatus.Scheduled };
            context.Matches.AddRange(match1, match2, match3);

            await context.SaveChangesAsync();

            var result = await locRepo.DeleteAsync(1, "user-1");
            Assert.True(result);

            Assert.Null(await context.Locations.FindAsync(1));
            Assert.Empty(await context.Boards.Where(b => b.LocationId == 1).ToListAsync());

            var updatedM1 = await context.Matches.FindAsync(100);
            var updatedM2 = await context.Matches.FindAsync(101);
            Assert.NotNull(updatedM1);
            Assert.Null(updatedM1.BoardId);
            Assert.NotNull(updatedM2);
            Assert.Null(updatedM2.BoardId);
        }

        [Fact]
        public async Task DeleteBoard_NullifiesMatches_WithoutDeletingThem()
        {
            using var context = CreateInMemoryContext("DeleteBoard_" + Guid.NewGuid());
            var boardRepo = new BoardRepository(context);

            var board = new Board { Id = 20, LocationId = 1, Label = "Board 20", UserId = "user-1", IsActive = true };
            context.Boards.Add(board);

            var match = new Match { Id = 200, BoardId = 20, Status = MatchStatus.InProgress };
            context.Matches.Add(match);
            await context.SaveChangesAsync();

            var result = await boardRepo.DeleteAsync(20, "user-1");
            Assert.True(result);

            Assert.Null(await context.Boards.FindAsync(20));
            var updatedMatch = await context.Matches.FindAsync(200);
            Assert.NotNull(updatedMatch);
            Assert.Null(updatedMatch.BoardId);
            Assert.Equal(MatchStatus.InProgress, updatedMatch.Status);
        }

        [Fact]
        public async Task DeleteTournament_CleansUpAllSubEntities()
        {
            using var context = CreateInMemoryContext("DeleteTournament_" + Guid.NewGuid());
            var matchRepo = new MatchRepository(context);
            var groupRepo = new GroupRepository(context, matchRepo);
            var braketRepo = new BraketRepository(context, matchRepo);
            var tourneyRepo = new TournamentRepository(context, groupRepo, braketRepo);

            var tourney = new Tournament { Id = 5, Name = "Grand Slam", UserId = "user-1", Code = "DART-SLAM01", StartDate = DateTimeOffset.UtcNow };
            context.Tournaments.Add(tourney);

            var config = new TournamentConfig { Id = 50, TournamentId = 5, GroupCount = 2 };
            context.TournamentConfigs.Add(config);

            var player = new Player { Id = 1, FirstName = "Phil", LastName = "Taylor", UserId = "user-1" };
            context.Players.Add(player);

            var tp = new TournamentPlayer { Id = 10, TournamentId = 5, PlayerId = 1 };
            context.TournamentPlayers.Add(tp);

            var group = new Group { Id = 20, TournamentId = 5, Name = "Gruppe A", Sequence = 1 };
            context.Groups.Add(group);

            var gp = new GroupPlayer { Id = 30, GroupId = 20, TournamentPlayerId = 10 };
            context.GroupPlayers.Add(gp);

            var braket = new Braket { Id = 40, TournamentId = 5, Sequence = 1 };
            context.Brakets.Add(braket);

            var match = new Match { Id = 50, BraketId = 40, GroupId = 20, Status = MatchStatus.Scheduled };
            context.Matches.Add(match);

            var mp = new MatchParticipant { Id = 60, MatchId = 50, TournamentPlayerId = 10 };
            context.MatchParticipants.Add(mp);

            await context.SaveChangesAsync();

            var result = await tourneyRepo.DeleteAsync(5);
            Assert.True(result);

            Assert.Null(await context.Tournaments.FindAsync(5));
            Assert.Empty(await context.Groups.Where(g => g.TournamentId == 5).ToListAsync());
            Assert.Empty(await context.Brakets.Where(b => b.TournamentId == 5).ToListAsync());
            Assert.Empty(await context.TournamentPlayers.Where(t => t.TournamentId == 5).ToListAsync());
            Assert.Empty(await context.Matches.Where(m => m.BraketId == 40).ToListAsync());
            Assert.Empty(await context.MatchParticipants.Where(m => m.MatchId == 50).ToListAsync());
            Assert.Empty(await context.GroupPlayers.Where(g => g.GroupId == 20).ToListAsync());

            // Player itself must NOT be deleted
            Assert.NotNull(await context.Players.FindAsync(1));
        }

        [Fact]
        public async Task DeletePlayer_InActiveTournament_ReturnsFalse_PreventingAccidentalDataLoss()
        {
            using var context = CreateInMemoryContext("DeletePlayerLocked_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);

            var player = new Player { Id = 99, FirstName = "Michael", LastName = "van Gerwen", UserId = "user-1" };
            context.Players.Add(player);

            var tourney = new Tournament { Id = 1, Name = "WM", UserId = "user-1", Code = "DART-WM0001", StartDate = DateTimeOffset.UtcNow };
            context.Tournaments.Add(tourney);

            var tp = new TournamentPlayer { Id = 500, TournamentId = 1, PlayerId = 99 };
            context.TournamentPlayers.Add(tp);
            await context.SaveChangesAsync();

            // When trying to delete player who is referenced by TournamentPlayer:
            // Since DeleteBehavior.Restrict is configured, EF Core/DB protects against deletion
            // PlayerRepository catches exception and returns false
            var deleted = await playerRepo.DeleteAsync(99, "user-1");
            Assert.False(deleted);
            Assert.NotNull(await context.Players.FindAsync(99));
        }
    }
}
