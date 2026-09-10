using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("TournamentConfigs")]
    public class TournamentConfig
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;
        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;
        public int MatchDurationMinutes { get; set; } = 30;
        public int BreakBetweenMatchesMinutes { get; set; } = 5;
        public int GroupCount { get; set; } = 1;
        public int? PlayersPerGroup { get; set; }
        public int QualifiersPerGroup { get; set; } = 1;
    }
}
