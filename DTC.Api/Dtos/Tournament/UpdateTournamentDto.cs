using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Tournament
{
    public class UpdateTournamentDto
    {
        [Required(ErrorMessage = "Der Name ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Die Beschreibung darf maximal 1000 Zeichen lang sein.")]
        public string? Description { get; set; }

        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;

        [Required(ErrorMessage = "Das Startdatum ist ein Pflichtfeld.")]
        public DateTimeOffset StartDate { get; set; }
    }
}
