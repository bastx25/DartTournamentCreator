using DTC.Api.Data;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Enums;
using DTC.Api.Models;
using DTC.Api.Repositories;
using DTC.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DTC.Api.Tests
{
    public class KnockoutServiceTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        private MatchMakerService CreateService(DartDbContext context)
        {
            var matchRepo = new MatchRepository(context);
            var tpRepo = new TournamentPlayerRepository(context);
            var groupRepo = new GroupRepository(context, matchRepo);
            var braketRepo = new BraketRepository(context, matchRepo);
            var boardService = new BoardService(context);
            return new MatchMakerService(context, tpRepo, matchRepo, groupRepo, braketRepo, boardService);
        }

        [Fact]
        public async Task AdvanceWinner_MovesWinnerToNextBraketTargetMatch()
        {
            using var context = CreateInMemoryContext("AdvanceWinnerTest_" + Guid.NewGuid());

            var tournament = new Tournament
            {
                Id = 1,
                Name = "Test Tournament",
                StartDate = DateTimeOffset.UtcNow,
                Status = TournamentStatus.InProgress
            };
            context.Tournaments.Add(tournament);

            var p1 = new Player { Id = 10, FirstName = "P1", LastName = "One" };
            var p2 = new Player { Id = 20, FirstName = "P2", LastName = "Two" };
            context.Players.AddRange(p1, p2);

            var tp1 = new TournamentPlayer { Id = 100, TournamentId = 1, PlayerId = 10, Player = p1 };
            var tp2 = new TournamentPlayer { Id = 200, TournamentId = 1, PlayerId = 20, Player = p2 };
            context.TournamentPlayers.AddRange(tp1, tp2);

            // Bracket 1: Halbfinale (Sequence 2)
            var semiBracket = new Braket
            {
                Id = 10,
                TournamentId = 1,
                Sequence = 2,
                Phase = BraketPhase.Knockout,
                Name = "Halbfinale"
            };

            // Bracket 2: Finale (Sequence 1)
            var finalBracket = new Braket
            {
                Id = 20,
                TournamentId = 1,
                Sequence = 1,
                Phase = BraketPhase.Knockout,
                Name = "Finale"
            };
            context.Brakets.AddRange(semiBracket, finalBracket);

            // Match 0 in Halbfinale
            var semiMatch0 = new Match
            {
                Id = 101,
                BraketId = 10,
                Status = MatchStatus.Completed
            };
            var part1 = new MatchParticipant
            {
                Id = 1,
                MatchId = 101,
                TournamentPlayerId = 100,
                TournamentPlayer = tp1,
                Score = 3,
                IsWinner = true
            };
            var part2 = new MatchParticipant
            {
                Id = 2,
                MatchId = 101,
                TournamentPlayerId = 200,
                TournamentPlayer = tp2,
                Score = 1,
                IsWinner = false
            };
            semiMatch0.Participants.Add(part1);
            semiMatch0.Participants.Add(part2);

            // Match 1 in Halbfinale
            var semiMatch1 = new Match
            {
                Id = 102,
                BraketId = 10,
                Status = MatchStatus.Scheduled
            };

            // Final Match in Finale
            var finalMatch = new Match
            {
                Id = 201,
                BraketId = 20,
                Status = MatchStatus.Scheduled
            };

            context.Matches.AddRange(semiMatch0, semiMatch1, finalMatch);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            // Act: Advance winner from semiMatch0
            await service.AdvanceWinner(semiMatch0);

            // Assert: finalMatch should now contain tp1
            var updatedFinalMatch = await context.Matches
                .Include(m => m.Participants)
                .FirstAsync(m => m.Id == 201);

            Assert.Single(updatedFinalMatch.Participants);
            Assert.Equal(100, updatedFinalMatch.Participants.First().TournamentPlayerId);
        }

        [Fact]
        public async Task AdvanceWinner_DoesNotDuplicateWinnerIfCalledTwice()
        {
            using var context = CreateInMemoryContext("AdvanceWinnerDuplicateTest_" + Guid.NewGuid());

            var tournament = new Tournament { Id = 2, Name = "T2", StartDate = DateTimeOffset.UtcNow, Status = TournamentStatus.InProgress };
            context.Tournaments.Add(tournament);

            var p1 = new Player { Id = 11, FirstName = "P11", LastName = "Eleven" };
            context.Players.Add(p1);
            var tp1 = new TournamentPlayer { Id = 101, TournamentId = 2, PlayerId = 11, Player = p1 };
            context.TournamentPlayers.Add(tp1);

            var b1 = new Braket { Id = 30, TournamentId = 2, Sequence = 2, Phase = BraketPhase.Knockout, Name = "Halbfinale" };
            var b2 = new Braket { Id = 40, TournamentId = 2, Sequence = 1, Phase = BraketPhase.Knockout, Name = "Finale" };
            context.Brakets.AddRange(b1, b2);

            var semiMatch = new Match { Id = 301, BraketId = 30, Status = MatchStatus.Completed };
            semiMatch.Participants.Add(new MatchParticipant { Id = 10, MatchId = 301, TournamentPlayerId = 101, TournamentPlayer = tp1, Score = 3, IsWinner = true });
            var finalMatch = new Match { Id = 401, BraketId = 40, Status = MatchStatus.Scheduled };

            context.Matches.AddRange(semiMatch, finalMatch);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            // Call twice
            await service.AdvanceWinner(semiMatch);
            await service.AdvanceWinner(semiMatch);

            var updatedFinalMatch = await context.Matches
                .Include(m => m.Participants)
                .FirstAsync(m => m.Id == 401);

            Assert.Single(updatedFinalMatch.Participants);
        }

        [Fact]
        public async Task AdvanceKnockoutAsync_LoadsMatchAndAdvancesWinner()
        {
            using var context = CreateInMemoryContext("AdvanceKnockoutAsyncTest_" + Guid.NewGuid());

            var tournament = new Tournament { Id = 3, Name = "T3", StartDate = DateTimeOffset.UtcNow, Status = TournamentStatus.InProgress };
            context.Tournaments.Add(tournament);

            var p1 = new Player { Id = 12, FirstName = "P12", LastName = "Twelve" };
            context.Players.Add(p1);
            var tp1 = new TournamentPlayer { Id = 102, TournamentId = 3, PlayerId = 12, Player = p1 };
            context.TournamentPlayers.Add(tp1);

            var b1 = new Braket { Id = 50, TournamentId = 3, Sequence = 2, Phase = BraketPhase.Knockout, Name = "Halbfinale" };
            var b2 = new Braket { Id = 60, TournamentId = 3, Sequence = 1, Phase = BraketPhase.Knockout, Name = "Finale" };
            context.Brakets.AddRange(b1, b2);

            var semiMatch = new Match { Id = 501, BraketId = 50, Status = MatchStatus.Completed };
            semiMatch.Participants.Add(new MatchParticipant { Id = 11, MatchId = 501, TournamentPlayerId = 102, TournamentPlayer = tp1, Score = 3, IsWinner = true });
            var finalMatch = new Match { Id = 601, BraketId = 60, Status = MatchStatus.Scheduled };

            context.Matches.AddRange(semiMatch, finalMatch);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            // Act: Call AdvanceKnockoutAsync by matchId
            await service.AdvanceKnockoutAsync(501);

            var updatedFinalMatch = await context.Matches
                .Include(m => m.Participants)
                .FirstAsync(m => m.Id == 601);

            Assert.Single(updatedFinalMatch.Participants);
            Assert.Equal(102, updatedFinalMatch.Participants.First().TournamentPlayerId);
        }
    }
}
