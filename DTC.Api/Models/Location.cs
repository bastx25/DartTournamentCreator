using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Locations")]
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // z. B. "Sporthalle Freistadt"
        public string? Address { get; set; }

        
        public ICollection<Board> Boards { get; set; } = new List<Board>();

    }
}
