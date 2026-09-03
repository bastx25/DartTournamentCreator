using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Groups")]
    public class Group
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int BoardId { get; set; }
        public int Sequence { get; set; }
        public string Name { get; set; } = string.Empty;
        public int QualifiersCount { get; set; }

        public Tournament Tournament { get; set; } = null!;
        public Board Board { get; set; } = null!;
        public ICollection<GroupPlayer> Players { get; set; } = new List<GroupPlayer>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
