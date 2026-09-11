using DTC.Api.Dtos.Match;
using DTC.Api.Enums;

namespace DTC.Api.Dtos.Braket
{
    public class BraketDto
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public int Sequence { get; set; }

        public string? Name { get; set; }

        public DateTimeOffset PlannedStart { get; set; }

        public DateTimeOffset? PlannedEnd { get; set; }

        public BraketStatus Status { get; set; }

        public BraketPhase Phase { get; set; }

    }
}
