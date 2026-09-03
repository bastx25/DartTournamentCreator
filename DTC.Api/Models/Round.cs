using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Rounds")]
    public class Round
    {
        public int Id { get; set; }
        public int TournamentId { get; set; } // Foreign Key (Required)
        public int LocationId { get; set; }   // Foreign Key (Required)

        public int Sequence { get; set; }     // Runden-Reihenfolge (1, 2, 3...)
        public string? Name { get; set; }     // Optional: "Achtelfinale", "Runde 1"

        public DateTimeOffset PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }

        public RoundStatus Status { get; set; } = RoundStatus.Scheduled;
        public RoundPhase Phase { get; set; } = RoundPhase.GroupStage;

        // Navigation Properties
        public Tournament Tournament { get; set; } = null!;
        public Location Location { get; set; } = null!;
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
