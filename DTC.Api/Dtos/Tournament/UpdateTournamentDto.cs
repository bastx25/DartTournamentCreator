using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Tournament
{
    public class UpdateTournamentDto
    {
        [Required(ErrorMessage = "Turniername ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        public DateTimeOffset StartDate { get; set; }

        [StringLength(500, ErrorMessage = "Die Beschreibung darf maximal 500 Zeichen lang sein.")]
        public string? Description { get; set; }

        public TournamentMode Mode { get; set; }
        public TournamentStatus Status { get; set; }
        public int? MatchDurationMinutes { get; set; }
        public int? BreakBetweenMatchesMinutes { get; set; }
    }
}
