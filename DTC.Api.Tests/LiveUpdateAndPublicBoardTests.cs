using DTC.Api.Controllers;
using DTC.Api.Data;
using DTC.Api.Dtos.Board;
using DTC.Api.Dtos.Location;
using DTC.Api.Dtos.Player;
using DTC.Api.Enums;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using DTC.Api.Repositories;
using DTC.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace DTC.Api.Tests
{
    public class LiveUpdateAndPublicBoardTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        private void SetUserContext(ControllerBase controller, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task Scenario1_BoardController_Update_ReturnsUpdatedBoardDto_WithNewLabel()
        {
            using var context = CreateInMemoryContext("BoardUpdateReturnDto_" + Guid.NewGuid());
            var boardRepo = new BoardRepository(context);
            var authService = new TournamentAuthorizationService(context);

            context.Locations.Add(new Location { Id = 1, Name = "Main Hall", UserId = "user-1" });
            context.Boards.Add(new Board { Id = 10, LocationId = 1, Label = "Board A", UserId = "user-1", IsActive = true });
            await context.SaveChangesAsync();

            var controller = new BoardController(boardRepo, authService);
            SetUserContext(controller, "user-1");

            var updateDto = new UpdateBoardDto
            {
                LocationId = 1,
                Label = "Board Links",
                IsActive = true
            };

            var actionResult = await controller.Update(10, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnedBoard = Assert.IsType<BoardDto>(okResult.Value);

            Assert.Equal(10, returnedBoard.Id);
            Assert.Equal("Board Links", returnedBoard.Label);
            Assert.True(returnedBoard.IsActive);

            var dbBoard = await context.Boards.FindAsync(10);
            Assert.Equal("Board Links", dbBoard!.Label);
        }

        [Fact]
        public async Task Scenario2_LocationController_Update_ReturnsUpdatedLocationDto()
        {
            using var context = CreateInMemoryContext("LocUpdateReturnDto_" + Guid.NewGuid());
            var locRepo = new LocationRepository(context);

            context.Locations.Add(new Location { Id = 5, Name = "Old Club", Address = "Street 1", UserId = "user-1" });
            await context.SaveChangesAsync();

            var controller = new LocationController(locRepo);
            SetUserContext(controller, "user-1");

            var updateDto = new UpdateLocationDto
            {
                Name = "New Arena",
                Address = "Street 2"
            };

            var actionResult = await controller.Update(5, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnedLoc = Assert.IsType<LocationDto>(okResult.Value);

            Assert.Equal(5, returnedLoc.Id);
            Assert.Equal("New Arena", returnedLoc.Name);
            Assert.Equal("Street 2", returnedLoc.Address);
        }

        [Fact]
        public async Task Scenario3_PlayerController_Update_ReturnsUpdatedPlayerDto()
        {
            using var context = CreateInMemoryContext("PlayerUpdateReturnDto_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);

            context.Players.Add(new Player { Id = 7, FirstName = "John", LastName = "Doe", UserId = "user-1" });
            await context.SaveChangesAsync();

            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var updateDto = new UpdatePlayerDto
            {
                FirstName = "Johnny",
                LastName = "Darts",
                Nickname = "The Rocket"
            };

            var actionResult = await controller.UpdatePlayer(7, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var returnedPlayer = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal(7, returnedPlayer.Id);
            Assert.Equal("Johnny", returnedPlayer.FirstName);
            Assert.Equal("Darts", returnedPlayer.LastName);
            Assert.Equal("The Rocket", returnedPlayer.Nickname);
        }

        [Fact]
        public async Task Scenario4_And_5_And_6_PublicBoards_AreViewableByAnyone_WithoutUserIds()
        {
            using var context = CreateInMemoryContext("PublicBoardsViewing_" + Guid.NewGuid());

            context.Locations.Add(new Location { Id = 1, Name = "Arena West", UserId = "user-B" });
            context.Boards.AddRange(
                new Board { Id = 101, LocationId = 1, Label = "Board A", UserId = "user-A", IsActive = true },
                new Board { Id = 102, LocationId = 1, Label = "Board B", UserId = "user-B", IsActive = true },
                new Board { Id = 103, LocationId = 1, Label = "Board Links", UserId = "user-B", IsActive = false }
            );
            await context.SaveChangesAsync();

            var controller = new BoardAccessController(context, null!);

            // Unauthenticated public request
            var actionResult = await controller.GetAllPublicBoards();
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var publicBoards = Assert.IsAssignableFrom<IEnumerable<PublicBoardDto>>(okResult.Value).ToList();

            Assert.Equal(3, publicBoards.Count);

            // User A can see User B's board in public list
            var boardB = publicBoards.FirstOrDefault(b => b.Id == 102);
            Assert.NotNull(boardB);
            Assert.Equal("Board B", boardB.Label);
            Assert.Equal("Arena West", boardB.LocationName);
            Assert.True(boardB.IsActive);

            // Verify PublicBoardDto does not have UserId property
            var dtoProperties = typeof(PublicBoardDto).GetProperties().Select(p => p.Name);
            Assert.DoesNotContain("UserId", dtoProperties);
        }

        [Fact]
        public async Task Scenario7_And_8_UserA_CannotEditOrDelete_UserB_Board()
        {
            using var context = CreateInMemoryContext("UserACannotEditDeleteBBoard_" + Guid.NewGuid());
            var boardRepo = new BoardRepository(context);
            var authService = new TournamentAuthorizationService(context);

            context.Locations.Add(new Location { Id = 2, Name = "Loc B", UserId = "user-B" });
            context.Boards.Add(new Board { Id = 77, LocationId = 2, Label = "Board B", UserId = "user-B", IsActive = true });
            await context.SaveChangesAsync();

            var controller = new BoardController(boardRepo, authService);
            SetUserContext(controller, "user-A");

            // Attempt to edit User B's board
            var updateResult = await controller.Update(77, new UpdateBoardDto { LocationId = 2, Label = "Hijacked", IsActive = true });
            Assert.IsType<NotFoundObjectResult>(updateResult);

            // Attempt to delete User B's board
            var deleteResult = await controller.Delete(77);
            Assert.IsType<NotFoundObjectResult>(deleteResult);

            // Verify board remains untouched
            var board = await context.Boards.FindAsync(77);
            Assert.NotNull(board);
            Assert.Equal("Board B", board.Label);
            Assert.Equal("user-B", board.UserId);
        }

        [Fact]
        public async Task Scenario9_PublicBoardScoring_FinishesMatchSuccessfully()
        {
            using var context = CreateInMemoryContext("PublicScoringEndToEnd_" + Guid.NewGuid());

            var board = new Board { Id = 1, LocationId = 1, Label = "Stage Board", UserId = "user-A", IsActive = true };
            context.Boards.Add(board);

            var p1 = new Player { Id = 1, FirstName = "P1", LastName = "One", UserId = "user-A" };
            var p2 = new Player { Id = 2, FirstName = "P2", LastName = "Two", UserId = "user-A" };
            context.Players.AddRange(p1, p2);

            var t = new Tournament { Id = 1, Name = "Cup", Code = "DART-TEST01", UserId = "user-A", StartDate = DateTimeOffset.UtcNow };
            context.Tournaments.Add(t);

            var tp1 = new TournamentPlayer { Id = 10, TournamentId = 1, PlayerId = 1, Player = p1 };
            var tp2 = new TournamentPlayer { Id = 11, TournamentId = 1, PlayerId = 2, Player = p2 };
            context.TournamentPlayers.AddRange(tp1, tp2);

            var match = new Match
            {
                Id = 100,
                BoardId = 1,
                Status = MatchStatus.InProgress,
                Participants = new List<MatchParticipant>
                {
                    new MatchParticipant { Id = 1, TournamentPlayerId = 10, TournamentPlayer = tp1, Score = 0 },
                    new MatchParticipant { Id = 2, TournamentPlayerId = 11, TournamentPlayer = tp2, Score = 0 }
                }
            };
            context.Matches.Add(match);
            await context.SaveChangesAsync();

            var controller = new BoardAccessController(context, null!);

            var finishDto = new FinishBoardMatchDto
            {
                Participants = new List<BoardParticipantScoreDto>
                {
                    new BoardParticipantScoreDto { ParticipantId = 1, Score = 3 },
                    new BoardParticipantScoreDto { ParticipantId = 2, Score = 0 }
                }
            };

            var result = await controller.FinishMatch(1, 100, finishDto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var matchDto = Assert.IsType<BoardMatchDto>(okResult.Value);

            Assert.Equal(MatchStatus.Completed, matchDto.Status);

            var updated = await context.Matches.Include(m => m.Participants).FirstAsync(m => m.Id == 100);
            Assert.Equal(MatchStatus.Completed, updated.Status);
            Assert.True(updated.Participants.First(p => p.Id == 1).IsWinner);
        }

        [Fact]
        public async Task Scenario10_PublicTournamentLookup_ValidAndInvalidCodes()
        {
            using var context = CreateInMemoryContext("PublicTournamentLookup_" + Guid.NewGuid());
            var service = new PublicTournamentService(context);
            var matchRepo = new MatchRepository(context);

            context.Tournaments.Add(new Tournament
            {
                Id = 10,
                Name = "Open Tournament",
                Code = "DART-OPEN01",
                Status = TournamentStatus.InProgress,
                UserId = "user-A",
                StartDate = DateTimeOffset.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = new PublicTournamentController(service, matchRepo);

            // Valid code
            var validRes = await controller.GetByCode("DART-OPEN01");
            var okValid = Assert.IsType<OkObjectResult>(validRes.Result);
            Assert.NotNull(okValid.Value);

            // Invalid code
            var invalidRes = await controller.GetByCode("DART-NONEXISTENT");
            Assert.IsType<NotFoundObjectResult>(invalidRes.Result);
        }
    }
}
