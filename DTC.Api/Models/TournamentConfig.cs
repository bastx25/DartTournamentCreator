using DTC.Api.Enums;

namespace DTC.Api.Models
{
    public class TournamentConfig
    {
        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;
        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
        public int MatchDurationMinutes { get; set; } = 30;
        public int BreakBetweenMatchesMinutes { get; set; } = 5;
        public int GroupCount { get; set; } = 1;
        public int? PlayersPerGroup { get; set; }

        public int QualifiersPerGroup { get; set; } = 1;
    }
}
