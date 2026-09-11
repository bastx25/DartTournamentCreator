using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Brakets")]
    public class Braket
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        public int Sequence { get; set; }

        public string? Name { get; set; }

        public DateTimeOffset PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }

        public BraketStatus Status { get; set; } = BraketStatus.Scheduled;

        public BraketPhase Phase { get; set; } = BraketPhase.GroupStage;

        public ICollection<Match> Matches { get; set; }
            = new List<Match>();
    }
}