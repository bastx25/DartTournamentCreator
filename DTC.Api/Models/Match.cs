using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Matches")]
    public class Match
    {
        public int Id { get; set; }
        public int RoundId { get; set; }  // Foreign Key (Required)
        public int? GroupId { get; set; }  // Foreign Key (Optional for knockout matches)
        public int? BoardId { get; set; } // Foreign Key (Optional / Nullable)

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;
        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
        public DateTimeOffset? ActualStart { get; set; }
        public DateTimeOffset? ActualEnd { get; set; }

        // Navigation Properties
        public Round Round { get; set; } = null!;
        public Group? Group { get; set; }
        public Board? Board { get; set; }
        public ICollection<MatchParticipant> Participants { get; set; } = new List<MatchParticipant>();
    }
}
