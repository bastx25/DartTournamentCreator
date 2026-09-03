using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Round
{
    public class CreateRoundDto
    {
        [Required(ErrorMessage = "TournamentId ist ein Pflichtfeld.")]
        public int TournamentId { get; set; }

        [Required(ErrorMessage = "LocationId ist ein Pflichtfeld.")]
        public int LocationId { get; set; }

        [Range(1, 100, ErrorMessage = "Die Sequenz muss mindestens 1 sein.")]
        public int Sequence { get; set; }

        [StringLength(50, ErrorMessage = "Der Rundenname darf maximal 50 Zeichen lang sein.")]
        public string? Name { get; set; }

        public DateTimeOffset PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
        public RoundStatus Status { get; set; } = RoundStatus.Scheduled;
        public RoundPhase Phase { get; set; } = RoundPhase.GroupStage;
    }
}
