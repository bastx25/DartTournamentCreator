using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Match
{
    public class UpdateMatchDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Die Round ist ein Pflichtfeld.")]
        public int RoundId { get; set; }

        // Nur bei Gruppenspielen gesetzt
        public int? GroupId { get; set; }

        // Board ist optional
        public int? BoardId { get; set; }

        public MatchStatus Status { get; set; }

        public DateTimeOffset? PlannedStart { get; set; }

        public DateTimeOffset? PlannedEnd { get; set; }

        public DateTimeOffset? ActualStart { get; set; }

        public DateTimeOffset? ActualEnd { get; set; }
    }
}
