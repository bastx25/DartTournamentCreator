using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Location
{
    public class CreateLocationDto
    {
        [Required(ErrorMessage = "Der Name des Standorts ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Name des Standorts darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Die Adresse darf maximal 250 Zeichen lang sein.")]
        public string? Address { get; set; }
    }
}
