using System.Text.RegularExpressions;
using DTC.Api.Data;
using DTC.Api.Models;
using DTC.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DTC.Api.Tests
{
    public class TournamentCodeGeneratorTests
    {
        private DartDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DartDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new DartDbContext(options);
        }

        [Fact]
        public async Task GenerateUniqueTournamentCode_MatchesExpectedFormat()
        {
            using var context = CreateInMemoryContext("CodeFormatTest_" + Guid.NewGuid());
            var generator = new TournamentCodeGenerator(context);

            var code = await generator.GenerateUniqueTournamentCodeAsync();

            Assert.NotNull(code);
            Assert.StartsWith("DART-", code);
            Assert.Equal(11, code.Length); // DART- + 6 characters

            // Verify characters come only from unambiguous charset
            var regex = new Regex(@"^DART-[23456789ABCDEFGHJKMNPQRSTUVWXYZ]{6}$");
            Assert.Matches(regex, code);

            // Verify no easily confused characters (O, 0, I, 1, L)
            Assert.DoesNotContain("O", code);
            Assert.DoesNotContain("0", code);
            Assert.DoesNotContain("I", code);
            Assert.DoesNotContain("1", code);
            Assert.DoesNotContain("L", code);
        }

        [Fact]
        public async Task GenerateUniqueTournamentCode_GeneratesUniqueCodes()
        {
            using var context = CreateInMemoryContext("CodeUniquenessTest_" + Guid.NewGuid());
            var generator = new TournamentCodeGenerator(context);

            var codes = new HashSet<string>();
            for (int i = 0; i < 50; i++)
            {
                var code = await generator.GenerateUniqueTournamentCodeAsync();
                Assert.DoesNotContain(code, codes);
                codes.Add(code);

                // Add to DB to simulate real environment
                context.Tournaments.Add(new Tournament
                {
                    Id = i + 1,
                    Name = $"Tournament {i}",
                    Code = code,
                    StartDate = DateTimeOffset.UtcNow
                });
                await context.SaveChangesAsync();
            }

            Assert.Equal(50, codes.Count);
        }
    }
}
