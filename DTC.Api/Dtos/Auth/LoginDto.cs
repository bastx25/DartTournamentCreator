using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-Mail ist ein Pflichtfeld.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passwort ist ein Pflichtfeld.")]
        public string Password { get; set; } = string.Empty;
    }
}
