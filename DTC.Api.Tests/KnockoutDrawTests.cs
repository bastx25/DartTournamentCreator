using DTC.Api.Enums;
using DTC.Api.Models;
using DTC.Api.Services;
using Xunit;

namespace DTC.Api.Tests
{
    public class KnockoutDrawTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        public void GenerateBracketSeedPairings_AllSeedsPresentExactlyOnce(int bracketSize)
        {
            var pairings = MatchMakerService.GenerateBracketSeedPairings(bracketSize);

            Assert.Equal(bracketSize / 2, pairings.Count);

            var allSeeds = pairings.SelectMany(p => new[] { p.Seed1, p.Seed2 }).ToList();
            Assert.Equal(bracketSize, allSeeds.Count);

            var expected = Enumerable.Range(1, bracketSize).ToList();
            Assert.Equal(expected.OrderBy(x => x), allSeeds.OrderBy(x => x));
        }

        [Fact]
        public void GenerateBracketSeedPairings_8PlayerBracket_Seeds1And2AreInOppositeHalves()
        {
            var pairings = MatchMakerService.GenerateBracketSeedPairings(8);

            // Match 0: 1 vs 8
            // Match 1: 4 vs 5
            // Match 2: 2 vs 7
            // Match 3: 3 vs 6
            Assert.Equal((1, 8), pairings[0]);
            Assert.Equal((4, 5), pairings[1]);
            Assert.Equal((2, 7), pairings[2]);
            Assert.Equal((3, 6), pairings[3]);

            // Top half: matches 0 & 1 -> seeds 1, 8, 4, 5
            // Bottom half: matches 2 & 3 -> seeds 2, 7, 3, 6
            var topHalfSeeds = new[] { pairings[0].Seed1, pairings[0].Seed2, pairings[1].Seed1, pairings[1].Seed2 };
            var bottomHalfSeeds = new[] { pairings[2].Seed1, pairings[2].Seed2, pairings[3].Seed1, pairings[3].Seed2 };

            Assert.Contains(1, topHalfSeeds);
            Assert.DoesNotContain(2, topHalfSeeds);

            Assert.Contains(2, bottomHalfSeeds);
            Assert.DoesNotContain(1, bottomHalfSeeds);
        }

        [Fact]
        public void GenerateBracketSeedPairings_16PlayerBracket_Seeds1And2AreInOppositeHalves()
        {
            var pairings = MatchMakerService.GenerateBracketSeedPairings(16);

            // First 4 matches feed into Semifinal 0 (top half)
            var topHalf = pairings.Take(4).SelectMany(p => new[] { p.Seed1, p.Seed2 }).ToList();
            // Next 4 matches feed into Semifinal 1 (bottom half)
            var bottomHalf = pairings.Skip(4).Take(4).SelectMany(p => new[] { p.Seed1, p.Seed2 }).ToList();

            Assert.Contains(1, topHalf);
            Assert.DoesNotContain(2, topHalf);

            Assert.Contains(2, bottomHalf);
            Assert.DoesNotContain(1, bottomHalf);
        }

        [Fact]
        public void GroupPlayerResult_TiebreakerEvaluation_LegDiffAndWins()
        {
            var p1 = new TournamentPlayer { Id = 1, Player = new Player { FirstName = "Alice", LastName = "A" } };
            var p2 = new TournamentPlayer { Id = 2, Player = new Player { FirstName = "Bob", LastName = "B" } };

            // p1: 2 wins, 6 legs won, 2 lost -> diff +4
            var r1 = new MatchMakerService.GroupPlayerResult(p1, 2, 6, 2, 2, 1, 1);
            // p2: 2 wins, 5 legs won, 4 lost -> diff +1
            var r2 = new MatchMakerService.GroupPlayerResult(p2, 2, 5, 4, 2, 1, 2);

            Assert.True(r1.LegDiff > r2.LegDiff);
            Assert.Equal(4, r1.LegDiff);
            Assert.Equal(1, r2.LegDiff);
        }

        [Fact]
        public void ByesAssignment_For6PlayersIn8Bracket_TopTwoSeedsGetByes()
        {
            var bracketSize = 8;
            var qualifiedCount = 6;
            var pairings = MatchMakerService.GenerateBracketSeedPairings(bracketSize);

            // Seeds 1..6 exist, seeds 7..8 are Byes (> 6)
            var matchesWithByes = new List<(int MatchIndex, int ByeSeed, int AdvancingSeed)>();

            for (int i = 0; i < pairings.Count; i++)
            {
                var (s1, s2) = pairings[i];
                if (s1 > qualifiedCount)
                    matchesWithByes.Add((i, s1, s2));
                else if (s2 > qualifiedCount)
                    matchesWithByes.Add((i, s2, s1));
            }

            Assert.Equal(2, matchesWithByes.Count);

            // Bye in Match 0 gives Bye to Seed 1
            Assert.Contains(matchesWithByes, m => m.MatchIndex == 0 && m.AdvancingSeed == 1);
            // Bye in Match 2 gives Bye to Seed 2
            Assert.Contains(matchesWithByes, m => m.MatchIndex == 2 && m.AdvancingSeed == 2);
        }

        [Fact]
        public void ByesAssignment_For3PlayersIn4Bracket_TopSeedGetsBye()
        {
            var bracketSize = 4;
            var qualifiedCount = 3;
            var pairings = MatchMakerService.GenerateBracketSeedPairings(bracketSize);

            // Pairings for 4: (1, 4), (2, 3)
            // Seed 4 is Bye (> 3)
            Assert.Equal((1, 4), pairings[0]);
            Assert.Equal((2, 3), pairings[1]);

            // Match 0 is Bye for Seed 1, Match 1 is regular match between Seed 2 and Seed 3
            var match0HasBye = pairings[0].Seed1 > qualifiedCount || pairings[0].Seed2 > qualifiedCount;
            var match1HasBye = pairings[1].Seed1 > qualifiedCount || pairings[1].Seed2 > qualifiedCount;

            Assert.True(match0HasBye);
            Assert.False(match1HasBye);
        }
    }
}
