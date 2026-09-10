using DTC.Api.Enums;

namespace DTC.Api.Dtos.TournamentConfig
{
    public class TournamentConfigDto
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public TournamentMode Mode { get; set; }

        public int MatchDurationMinutes { get; set; }

        public int BreakBetweenMatchesMinutes { get; set; }

        public int GroupCount { get; set; }

        public int? PlayersPerGroup { get; set; }

        public int QualifiersPerGroup { get; set; }
    }
}
