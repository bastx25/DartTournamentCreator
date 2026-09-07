using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Rounds")]
    public class Round
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public int Sequence { get; set; }

        public string? Name { get; set; }

        public DateTimeOffset PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }

        public RoundStatus Status { get; set; } = RoundStatus.Scheduled;

        public RoundPhase Phase { get; set; } = RoundPhase.GroupStage;

        // Navigation Properties
        public Tournament Tournament { get; set; } = null!;

        public ICollection<Match> Matches { get; set; }
            = new List<Match>();
    }
}