using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Groups")]
    public class Group
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;
        public int Sequence { get; set; }
        public string Name { get; set; } = string.Empty;
        public int QualifiersCount { get; set; }

        public ICollection<GroupPlayer> GroupPlayers { get; set; } = new List<GroupPlayer>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
