using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Tournaments")]
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public string? Description { get; set; }

        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;
        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;

        public int MatchDurationMinutes { get; set; } = 30;
        public int BreakBetweenMatchesMinutes { get; set; } = 5;

        // Navigation Properties
        public ICollection<Round> Rounds { get; set; } = new List<Round>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
