using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Locations")]
    public class Location
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string Name { get; set; } = string.Empty; // z. B. "Sporthalle Freistadt"
        public string? Address { get; set; }

        public ICollection<Board> Boards { get; set; } = new List<Board>();
    }
}
