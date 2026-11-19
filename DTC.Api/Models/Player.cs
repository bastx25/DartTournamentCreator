using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Players")]
    public class Player
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? Nickname { get; set; }

        public ICollection<TournamentPlayer> TournamentPlayers { get; set; }
            = new List<TournamentPlayer>();
    }
}
