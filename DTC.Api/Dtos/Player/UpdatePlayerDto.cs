using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Player
{
    public class UpdatePlayerDto
    {
        [Required(ErrorMessage = "Der Vorname ist ein Pflichtfeld.")]
        [StringLength(50, ErrorMessage = "Der Vorname darf maximal 50 Zeichen lang sein.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Der Nachname ist ein Pflichtfeld.")]
        [StringLength(50, ErrorMessage = "Der Nachname darf maximal 50 Zeichen lang sein.")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Der Spitzname darf maximal 50 Zeichen lang sein.")]
        public string? Nickname { get; set; }
    }
}
