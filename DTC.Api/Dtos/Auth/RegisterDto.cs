using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "E-Mail ist ein Pflichtfeld.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passwort ist ein Pflichtfeld.")]
        [MinLength(6, ErrorMessage = "Das Passwort muss mindestens 6 Zeichen lang sein.")]
        public string Password { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Vorname darf maximal 50 Zeichen lang sein.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Nachname darf maximal 50 Zeichen lang sein.")]
        public string? LastName { get; set; }
    }
}
