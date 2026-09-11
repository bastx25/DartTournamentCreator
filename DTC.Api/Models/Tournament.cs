using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Tournaments")]
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
        public DateTimeOffset StartDate { get; set; }
        public TournamentConfig Config { get; set; } = null!;

        public ICollection<TournamentPlayer> TournamentPlayers { get; set; }
            = new List<TournamentPlayer>();

        public ICollection<Group> Groups { get; set; }
            = new List<Group>();

        public ICollection<Braket> Brakets { get; set; }
            = new List<Braket>();
    }
}