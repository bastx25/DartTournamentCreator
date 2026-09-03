using DTC.Api.Dtos.Match;
using DTC.Api.Enums;

namespace DTC.Api.Dtos.Round
{
    public class RoundDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int LocationId { get; set; }
        public int Sequence { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
        public RoundStatus Status { get; set; }
        public RoundPhase Phase { get; set; }
        public List<MatchDto> Matches { get; set; } = new();
    }
}
