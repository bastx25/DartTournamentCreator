using DTC.Api.Controllers;
using DTC.Api.Data;
using DTC.Api.Dtos.Board;
using DTC.Api.Enums;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using DTC.Api.Repositories;
using DTC.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DTC.Api.Tests
{
    public class MultiTenantDataAndBoardTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        [Fact]
        public async Task Scenario1_UserA_SeesOnlyOwnTournaments()
        {
            using var context = CreateInMemoryContext("MultiTenantTournaments_" + Guid.NewGuid());
            var repo = new TournamentRepository(context);

            context.Tournaments.AddRange(
                new Tournament { Id = 1, Name = "User A Tourney", UserId = "user-A", Code = "DART-AAAA01", StartDate = DateTimeOffset.UtcNow },
                new Tournament { Id = 2, Name = "User B Tourney", UserId = "user-B", Code = "DART-BBBB02", StartDate = DateTimeOffset.UtcNow }
            );
            await context.SaveChangesAsync();

            var tournamentsA = await repo.GetByUserIdAsync("user-A");
            Assert.Single(tournamentsA);
            Assert.Equal("User A Tourney", tournamentsA[0].Name);

            var tournamentsB = await repo.GetByUserIdAsync("user-B");
            Assert.Single(tournamentsB);
            Assert.Equal("User B Tourney", tournamentsB[0].Name);
        }

        [Fact]
        public async Task Scenario2_UserA_SeesOnlyOwnPlayers()
        {
            using var context = CreateInMemoryContext("MultiTenantPlayers_" + Guid.NewGuid());
            var repo = new PlayerRepository(context);

            context.Players.AddRange(
                new Player { Id = 1, FirstName = "Max", LastName = "UserA", UserId = "user-A" },
                new Player { Id = 2, FirstName = "Erika", LastName = "UserB", UserId = "user-B" }
            );
            await context.SaveChangesAsync();

            var playersA = await repo.GetByUserIdAsync("user-A");
            Assert.Single(playersA);
            Assert.Equal("UserA", playersA[0].LastName);

            var playersB = await repo.GetByUserIdAsync("user-B");
            Assert.Single(playersB);
            Assert.Equal("UserB", playersB[0].LastName);
        }

        [Fact]
        public async Task Scenario3_UserA_SeesOnlyOwnLocations()
        {
            using var context = CreateInMemoryContext("MultiTenantLocations_" + Guid.NewGuid());
            var repo = new LocationRepository(context);

            context.Locations.AddRange(
                new Location { Id = 1, Name = "Clubhaus A", UserId = "user-A" },
                new Location { Id = 2, Name = "Gasthof B", UserId = "user-B" }
            );
            await context.SaveChangesAsync();

            var locsA = await repo.GetByUserIdAsync("user-A");
            Assert.Single(locsA);
            Assert.Equal("Clubhaus A", locsA.First().Name);

            var locsB = await repo.GetByUserIdAsync("user-B");
            Assert.Single(locsB);
            Assert.Equal("Gasthof B", locsB.First().Name);
        }

        [Fact]
        public async Task Scenario4_UserA_SeesOnlyOwnBoards()
        {
            using var context = CreateInMemoryContext("MultiTenantBoards_" + Guid.NewGuid());
            var repo = new BoardRepository(context);

            context.Boards.AddRange(
                new Board { Id = 1, LocationId = 10, Label = "Board A1", UserId = "user-A", IsActive = true },
                new Board { Id = 2, LocationId = 20, Label = "Board B1", UserId = "user-B", IsActive = true }
            );
            await context.SaveChangesAsync();

            var boardsA = await repo.GetByUserIdAsync("user-A");
            Assert.Single(boardsA);
            Assert.Equal("Board A1", boardsA.First().Label);

            var boardsB = await repo.GetByUserIdAsync("user-B");
            Assert.Single(boardsB);
            Assert.Equal("Board B1", boardsB.First().Label);
        }

        [Fact]
        public async Task Scenario5_UserA_CannotEditOrDelete_UserBData()
        {
            using var context = CreateInMemoryContext("NoCrossEditDelete_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);
            var locRepo = new LocationRepository(context);
            var boardRepo = new BoardRepository(context);

            context.Players.Add(new Player { Id = 10, FirstName = "Hans", LastName = "B", UserId = "user-B" });
            context.Locations.Add(new Location { Id = 20, Name = "Loc B", UserId = "user-B" });
            context.Boards.Add(new Board { Id = 30, LocationId = 20, Label = "Board B", UserId = "user-B" });
            await context.SaveChangesAsync();

            // User A attempts to delete User B's player, location, board
            Assert.False(await playerRepo.DeleteAsync(10, "user-A"));
            Assert.False(await locRepo.DeleteAsync(20, "user-A"));
            Assert.False(await boardRepo.DeleteAsync(30, "user-A"));

            // User A attempts to get User B's data
            Assert.Null(await playerRepo.GetByIdAndUserIdAsync(10, "user-A"));
            Assert.Null(await locRepo.GetByIdAndUserIdAsync(20, "user-A"));
            Assert.Null(await boardRepo.GetByIdAndUserIdAsync(30, "user-A"));
        }

        [Fact]
        public async Task Scenario6_UserA_CannotAddForeignPlayersToOwnTournament()
        {
            using var context = CreateInMemoryContext("NoForeignPlayerAdd_" + Guid.NewGuid());
            var authService = new TournamentAuthorizationService(context);

            context.Players.AddRange(
                new Player { Id = 1, FirstName = "PlayerA1", LastName = "A", UserId = "user-A" },
                new Player { Id = 2, FirstName = "PlayerB1", LastName = "B", UserId = "user-B" }
            );
            await context.SaveChangesAsync();

            // All players belong to user-A?
            Assert.True(await authService.HasAllPlayersAccessAsync(new[] { 1 }, "user-A"));
            Assert.False(await authService.HasAllPlayersAccessAsync(new[] { 1, 2 }, "user-A"));
            Assert.False(await authService.HasAllPlayersAccessAsync(new[] { 2 }, "user-A"));
        }

        [Fact]
        public async Task Scenario7_UserA_CannotUseForeignLocationForOwnBoard()
        {
            using var context = CreateInMemoryContext("NoForeignLocation_" + Guid.NewGuid());
            var authService = new TournamentAuthorizationService(context);

            context.Locations.Add(new Location { Id = 55, Name = "Location B", UserId = "user-B" });
            await context.SaveChangesAsync();

            Assert.False(await authService.HasLocationAccessAsync(55, "user-A"));
            Assert.True(await authService.HasLocationAccessAsync(55, "user-B"));
        }

        [Fact]
        public async Task Scenario8_UserA_CannotAssignForeignBoardsToTournament()
        {
            using var context = CreateInMemoryContext("NoForeignBoard_" + Guid.NewGuid());
            var boardService = new BoardService(context);

            context.Boards.Add(new Board { Id = 1, LocationId = 1, Label = "Board B", UserId = "user-B", IsActive = true });
            await context.SaveChangesAsync();

            var groups = new List<Group>
            {
                new Group { Id = 1, TournamentId = 10, Name = "Gruppe A" }
            };

            // Attempting to schedule matches for user-A when user-A has no boards throws exception
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                boardService.SetBoards(groups, DateTimeOffset.UtcNow, 30, 5, "user-A")
            );

            Assert.Contains("Dartboard", ex.Message);
        }

        [Fact]
        public async Task Scenario9_And_10_PublicBoardScoring_WorksWithoutLogin()
        {
            using var context = CreateInMemoryContext("PublicBoardScoring_" + Guid.NewGuid());

            var board = new Board { Id = 77, LocationId = 1, Label = "Dartboard 77", UserId = "user-A", IsActive = true };
            context.Boards.Add(board);

            var player1 = new Player { Id = 1, FirstName = "Anna", LastName = "Schmidt", UserId = "user-A" };
            var player2 = new Player { Id = 2, FirstName = "Ben", LastName = "Meier", UserId = "user-A" };
            context.Players.AddRange(player1, player2);

            var tourney = new Tournament { Id = 1, Name = "Cup", Code = "DART-CUP123", UserId = "user-A", StartDate = DateTimeOffset.UtcNow };
            context.Tournaments.Add(tourney);

            var tp1 = new TournamentPlayer { Id = 101, TournamentId = 1, PlayerId = 1, Player = player1 };
            var tp2 = new TournamentPlayer { Id = 102, TournamentId = 1, PlayerId = 2, Player = player2 };
            context.TournamentPlayers.AddRange(tp1, tp2);

            var match = new Match
            {
                Id = 500,
                BoardId = 77,
                Status = MatchStatus.InProgress,
                Participants = new List<MatchParticipant>
                {
                    new MatchParticipant { Id = 1001, TournamentPlayerId = 101, TournamentPlayer = tp1, Score = 0 },
                    new MatchParticipant { Id = 1002, TournamentPlayerId = 102, TournamentPlayer = tp2, Score = 0 }
                }
            };
            context.Matches.Add(match);
            await context.SaveChangesAsync();

            // Simulate anonymous match submission via BoardAccessController
            var controller = new BoardAccessController(context, null!);

            var finishDto = new FinishBoardMatchDto
            {
                Participants = new List<BoardParticipantScoreDto>
                {
                    new BoardParticipantScoreDto { ParticipantId = 1001, Score = 3 },
                    new BoardParticipantScoreDto { ParticipantId = 1002, Score = 1 }
                }
            };

            var response = await controller.FinishMatch(77, 500, finishDto);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var resultDto = Assert.IsType<BoardMatchDto>(okResult.Value);

            Assert.Equal(MatchStatus.Completed, resultDto.Status);

            var updatedMatch = await context.Matches.Include(m => m.Participants).FirstAsync(m => m.Id == 500);
            Assert.Equal(MatchStatus.Completed, updatedMatch.Status);
            Assert.True(updatedMatch.Participants.First(p => p.Id == 1001).IsWinner);
            Assert.False(updatedMatch.Participants.First(p => p.Id == 1002).IsWinner);
        }

        [Fact]
        public async Task Scenario12_13_14_MissingResourcesCount_CheckedAccurately()
        {
            using var context = CreateInMemoryContext("PrerequisitesCheck_" + Guid.NewGuid());
            var authService = new TournamentAuthorizationService(context);

            // User has nothing
            Assert.Equal(0, await authService.GetActiveBoardsCountAsync("new-user"));
            Assert.Equal(0, await authService.GetLocationsCountAsync("new-user"));

            // Add location only
            context.Locations.Add(new Location { Id = 1, Name = "Location 1", UserId = "new-user" });
            await context.SaveChangesAsync();
            Assert.Equal(0, await authService.GetActiveBoardsCountAsync("new-user"));
            Assert.Equal(1, await authService.GetLocationsCountAsync("new-user"));

            // Add board
            context.Boards.Add(new Board { Id = 1, LocationId = 1, Label = "Board 1", UserId = "new-user", IsActive = true });
            await context.SaveChangesAsync();
            Assert.Equal(1, await authService.GetActiveBoardsCountAsync("new-user"));
            Assert.Equal(1, await authService.GetLocationsCountAsync("new-user"));
        }

        [Fact]
        public void Scenario15_BoardDto_DoesNotRequireOrContain_Number()
        {
            var createDto = new CreateBoardDto
            {
                LocationId = 5,
                Label = "Test Board",
                IsActive = true
            };

            var entity = DTC.Api.Mappers.BoardMapper.ToEntityFromCreate(createDto);
            Assert.Equal(5, entity.LocationId);
            Assert.Equal("Test Board", entity.Label);
            Assert.True(entity.IsActive);

            entity.Id = 42;
            var boardDto = DTC.Api.Mappers.BoardMapper.ToBoardDto(entity);
            Assert.Equal(42, boardDto.Id);
            Assert.Equal("Test Board", boardDto.Label);
            Assert.True(boardDto.IsActive);
        }

        [Fact]
        public async Task Scenario16_PublicBoardAccess_RetrievesBoardById()
        {
            using var context = CreateInMemoryContext("PublicBoardLookup_" + Guid.NewGuid());
            context.Boards.Add(new Board { Id = 99, LocationId = 1, Label = "Arena Board", UserId = "user-A", IsActive = true });
            await context.SaveChangesAsync();

            var controller = new BoardAccessController(context, null!);
            var actionResult = await controller.GetBoard(99);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var boardDto = Assert.IsType<BoardDto>(okResult.Value);
            Assert.Equal(99, boardDto.Id);
            Assert.Equal("Arena Board", boardDto.Label);
        }
    }
}

