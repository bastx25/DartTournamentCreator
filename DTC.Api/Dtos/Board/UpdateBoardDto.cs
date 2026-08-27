using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Board
{
    public class UpdateBoardDto
    {
        [Range(1, 100, ErrorMessage = "Die Boardnummer muss mindestens 1 sein.")]
        public int Number { get; set; }

        [StringLength(50, ErrorMessage = "Das Label darf maximal 50 Zeichen lang sein.")]
        public string? Label { get; set; }

        public bool IsActive { get; set; }
    }
}
