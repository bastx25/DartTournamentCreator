using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Boards")]
    public class Board
    {
        public int Id { get; set; }
        public int LocationId { get; set; } // Foreign Key (Required)
        public int Number { get; set; }     // Board-Nummer (z.B. 1, 2, 3)
        public string? Label { get; set; }  // Optional: "Board A", "Show-Board"
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Location Location { get; set; } = null!;
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
