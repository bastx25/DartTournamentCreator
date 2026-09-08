using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Board
{
    public class CreateBoardDto
    {
        [Required(ErrorMessage = "Der Standort ist ein Pflichtfeld.")]
        public int LocationId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Die Board-Nummer muss größer als 0 sein.")]
        public int Number { get; set; }

        [StringLength(100, ErrorMessage = "Das Label darf maximal 100 Zeichen lang sein.")]
        public string? Label { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
