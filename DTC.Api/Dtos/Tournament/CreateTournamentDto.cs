using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Tournament
{
    public class CreateTournamentDto
    {
        [Required(ErrorMessage = "Turniername ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Startdatum ist ein Pflichtfeld.")]
        public DateTimeOffset StartDate { get; set; }

        [StringLength(500, ErrorMessage = "Die Beschreibung darf maximal 500 Zeichen lang sein.")]
        public string? Description { get; set; }

        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;
        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
        public int MatchDurationMinutes { get; set; } = 30;
        public int BreakBetweenMatchesMinutes { get; set; } = 5;
    }
}
