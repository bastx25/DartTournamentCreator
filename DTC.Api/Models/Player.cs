using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Players")]
    public class Player
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Nickname { get; set; }

        // Navigation Properties
        public ICollection<TournamentPlayer> TournamentPlayers { get; set; }
            = new List<TournamentPlayer>();
    }
}