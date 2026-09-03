using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Match
{
    public class CreateMatchDto
    {
        [Required(ErrorMessage = "RoundId ist ein Pflichtfeld.")]
        public int RoundId { get; set; }
        public int? GroupId { get; set; }

        public int? BoardId { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;
        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
    }
}
