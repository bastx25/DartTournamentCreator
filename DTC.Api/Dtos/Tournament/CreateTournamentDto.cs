using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Tournament
{
    public class CreateTournamentDto
    {
        [Required(ErrorMessage = "Der Turniername ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Turniername darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Das Startdatum ist ein Pflichtfeld.")]
        public DateTimeOffset StartDate { get; set; }

        [StringLength(1000, ErrorMessage = "Die Beschreibung darf maximal 1000 Zeichen lang sein.")]
        public string? Description { get; set; }

        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;

        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;

        [Range(1, 1440, ErrorMessage = "Die Matchdauer muss zwischen 1 und 1440 Minuten liegen.")]
        public int MatchDurationMinutes { get; set; } = 30;

        [Range(0, 1440, ErrorMessage = "Die Pause zwischen Matches muss zwischen 0 und 1440 Minuten liegen.")]
        public int BreakBetweenMatchesMinutes { get; set; } = 5;
    }

}
