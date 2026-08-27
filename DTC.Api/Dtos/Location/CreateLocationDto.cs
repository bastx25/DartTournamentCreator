using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Location
{
    public class CreateLocationDto
    {
        [Required(ErrorMessage = "Name ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Die Adresse darf maximal 200 Zeichen lang sein.")]
        public string? Address { get; set; }
    }
}
