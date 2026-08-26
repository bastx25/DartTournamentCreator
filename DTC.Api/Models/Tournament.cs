using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Tournaments")]
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public string? Description { get; set; }

        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;
        public TournamentStatus Status { get; set; } = TournamentStatus.Draft;

        // Navigation Properties
        public ICollection<Round> Rounds { get; set; } = new List<Round>();
    }
}
