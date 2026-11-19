using DTC.Api.Controllers;
using DTC.Api.Data;
using DTC.Api.Dtos.Player;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
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
    public class PlayerOptionalFieldsTests
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
        public async Task Case1_CreatePlayer_OnlyFirstName_Succeeds()
        {
            using var context = CreateInMemoryContext("CreatePlayer_OnlyFirstName_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);
            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var dto = new CreatePlayerDto
            {
                FirstName = "Lukas",
                LastName = null,
                Nickname = null
            };

            var result = await controller.CreatePlayer(dto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var created = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal("Lukas", created.FirstName);
            Assert.Null(created.LastName);
            Assert.Null(created.Nickname);
            Assert.Equal("Lukas", created.DisplayName);

            var dbPlayer = await context.Players.FindAsync(created.Id);
            Assert.NotNull(dbPlayer);
            Assert.Equal("Lukas", dbPlayer.FirstName);
            Assert.Null(dbPlayer.LastName);
            Assert.Null(dbPlayer.Nickname);
        }

        [Fact]
        public async Task Case2_CreatePlayer_FirstNameAndLastName_Succeeds()
        {
            using var context = CreateInMemoryContext("CreatePlayer_FirstAndLast_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);
            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var dto = new CreatePlayerDto
            {
                FirstName = "Lukas",
                LastName = "Huber",
                Nickname = null
            };

            var result = await controller.CreatePlayer(dto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var created = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal("Lukas", created.FirstName);
            Assert.Equal("Huber", created.LastName);
            Assert.Null(created.Nickname);
            Assert.Equal("Lukas Huber", created.DisplayName);
        }

        [Fact]
        public async Task Case3_CreatePlayer_FirstNameAndNickname_Succeeds()
        {
            using var context = CreateInMemoryContext("CreatePlayer_FirstAndNick_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);
            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var dto = new CreatePlayerDto
            {
                FirstName = "Lukas",
                LastName = null,
                Nickname = "Luigi"
            };

            var result = await controller.CreatePlayer(dto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var created = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal("Lukas", created.FirstName);
            Assert.Null(created.LastName);
            Assert.Equal("Luigi", created.Nickname);
            Assert.Equal("Lukas 'Luigi'", created.DisplayName);
        }

        [Fact]
        public async Task Case4_CreatePlayer_AllThreeFields_Succeeds()
        {
            using var context = CreateInMemoryContext("CreatePlayer_AllFields_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);
            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var dto = new CreatePlayerDto
            {
                FirstName = "Lukas",
                LastName = "Huber",
                Nickname = "Luigi"
            };

            var result = await controller.CreatePlayer(dto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var created = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal("Lukas", created.FirstName);
            Assert.Equal("Huber", created.LastName);
            Assert.Equal("Luigi", created.Nickname);
            Assert.Equal("Lukas 'Luigi' Huber", created.DisplayName);
        }

        [Fact]
        public async Task Case5_EditPlayer_RemoveLastName_Succeeds()
        {
            using var context = CreateInMemoryContext("EditPlayer_RemoveLast_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);

            context.Players.Add(new Player
            {
                Id = 1,
                FirstName = "Lukas",
                LastName = "Huber",
                Nickname = "Luigi",
                UserId = "user-1"
            });
            await context.SaveChangesAsync();

            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var updateDto = new UpdatePlayerDto
            {
                FirstName = "Lukas",
                LastName = "   ", // whitespace should be treated as null
                Nickname = "Luigi"
            };

            var actionResult = await controller.UpdatePlayer(1, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var updated = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal("Lukas", updated.FirstName);
            Assert.Null(updated.LastName);
            Assert.Equal("Luigi", updated.Nickname);
            Assert.Equal("Lukas 'Luigi'", updated.DisplayName);

            var dbPlayer = await context.Players.FindAsync(1);
            Assert.Null(dbPlayer!.LastName);
        }

        [Fact]
        public async Task Case6_EditPlayer_RemoveNickname_Succeeds()
        {
            using var context = CreateInMemoryContext("EditPlayer_RemoveNick_" + Guid.NewGuid());
            var playerRepo = new PlayerRepository(context);

            context.Players.Add(new Player
            {
                Id = 2,
                FirstName = "Lukas",
                LastName = "Huber",
                Nickname = "Luigi",
                UserId = "user-1"
            });
            await context.SaveChangesAsync();

            var controller = new PlayerController(playerRepo, null!);
            SetUserContext(controller, "user-1");

            var updateDto = new UpdatePlayerDto
            {
                FirstName = "Lukas",
                LastName = "Huber",
                Nickname = "" // empty string should be treated as null
            };

            var actionResult = await controller.UpdatePlayer(2, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var updated = Assert.IsType<PlayerDto>(okResult.Value);

            Assert.Equal("Lukas", updated.FirstName);
            Assert.Equal("Huber", updated.LastName);
            Assert.Null(updated.Nickname);
            Assert.Equal("Lukas Huber", updated.DisplayName);

            var dbPlayer = await context.Players.FindAsync(2);
            Assert.Null(dbPlayer!.Nickname);
        }

        [Theory]
        [InlineData("Max", null, null, "Max")]
        [InlineData("Max", "Mustermann", null, "Max Mustermann")]
        [InlineData("Max", null, "The Power", "Max 'The Power'")]
        [InlineData("Max", "Mustermann", "The Power", "Max 'The Power' Mustermann")]
        [InlineData("  Max  ", "  Mustermann  ", "  The Power  ", "Max 'The Power' Mustermann")]
        [InlineData("  Max  ", "   ", "   ", "Max")]
        public void Case7_DisplayNameFormatting_ProducesNoNullOrUndefined(
            string firstName,
            string? lastName,
            string? nickname,
            string expectedDisplayName)
        {
            var player = new Player
            {
                FirstName = firstName,
                LastName = lastName,
                Nickname = nickname
            };

            var dto = player.ToPlayerDto();

            Assert.Equal(expectedDisplayName, dto.DisplayName);
            Assert.DoesNotContain("null", dto.DisplayName);
            Assert.DoesNotContain("undefined", dto.DisplayName);
        }
    }
}
