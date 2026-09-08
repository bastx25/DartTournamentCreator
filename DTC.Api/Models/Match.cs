using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Matches")]
    public class Match
    {
        public int Id { get; set; }

        // Jedes Match gehört zu einer Round
        public int RoundId { get; set; }
        public Round Round { get; set; } = null!;

        // Nur bei Gruppenspielen gesetzt
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        // Board ist optional
        public int? BoardId { get; set; }
        public Board? Board { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }

        public DateTimeOffset? ActualStart { get; set; }
        public DateTimeOffset? ActualEnd { get; set; }

        public ICollection<MatchParticipant> Participants { get; set; }
            = new List<MatchParticipant>();
    }
}