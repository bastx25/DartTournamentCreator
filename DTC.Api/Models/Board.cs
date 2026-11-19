using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Boards")]
    public class Board
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int LocationId { get; set; } // Foreign Key (Required)
        public Location Location { get; set; } = null!;

        public string? Label { get; set; }  // "Board links", "Board A", etc.
        public bool IsActive { get; set; } = true;

        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
