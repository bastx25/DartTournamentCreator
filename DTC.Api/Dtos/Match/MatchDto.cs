using DTC.Api.Dtos.MatchParticipant;
using DTC.Api.Enums;

namespace DTC.Api.Dtos.Match
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int RoundId { get; set; }
        public int? GroupId { get; set; }
        public int? BoardId { get; set; }

        public MatchStatus Status { get; set; }
        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
        public DateTimeOffset? ActualStart { get; set; }
        public DateTimeOffset? ActualEnd { get; set; }

        public List<MatchParticipantDto> Participants { get; set; } = new();
    }
}
