using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Tournaments")]
    public class Tournament
    {
        public int Id { get; set; }
        public TournamentConfiguration Config { get; set; } = new TournamentConfiguration();
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
