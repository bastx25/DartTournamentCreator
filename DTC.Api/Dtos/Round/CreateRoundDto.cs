using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Round
{
    public class CreateRoundDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Das Turnier ist ein Pflichtfeld.")]
        public int TournamentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Die Reihenfolge muss größer als 0 sein.")]
        public int Sequence { get; set; }

        [StringLength(100, ErrorMessage = "Der Name der Runde darf maximal 100 Zeichen lang sein.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Der geplante Startzeitpunkt ist ein Pflichtfeld.")]
        public DateTimeOffset PlannedStart { get; set; }

        public DateTimeOffset? PlannedEnd { get; set; }

        public RoundStatus Status { get; set; } = RoundStatus.Scheduled;

        public RoundPhase Phase { get; set; } = RoundPhase.GroupStage;
    }
}
