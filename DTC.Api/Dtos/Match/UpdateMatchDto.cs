using DTC.Api.Enums;

namespace DTC.Api.Dtos.Match
{
    public class UpdateMatchDto
    {
        public int? BoardId { get; set; }
        public MatchStatus Status { get; set; }
        public DateTimeOffset? ActualStart { get; set; }
        public DateTimeOffset? ActualEnd { get; set; }
    }
}
