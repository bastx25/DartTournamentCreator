using DTC.Api.Dtos.MatchParticipant;
using DTC.Api.Enums;

namespace DTC.Api.Dtos.Board
{
    public class BoardMatchDto
    {
        public int MatchId { get; set; }
        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? ActualStart { get; set; }
        public MatchStatus Status { get; set; }
        public List<MatchParticipantDto> Participants { get; set; } = new();
    }
}
