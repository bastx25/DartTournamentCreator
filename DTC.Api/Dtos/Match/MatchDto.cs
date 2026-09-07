using DTC.Api.Dtos.Board;
using DTC.Api.Dtos.MatchParticipant;
using DTC.Api.Enums;
using DTC.Api.Models;
using System.Security.Cryptography.Pkcs;

namespace DTC.Api.Dtos.Match
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int RoundId { get; set; }
        public int? GroupId { get; set; }
        public BoardDto? Board { get; set; }

        public MatchStatus Status { get; set; }
        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
        public DateTimeOffset? ActualStart { get; set; }
        public DateTimeOffset? ActualEnd { get; set; }

        public List<MatchParticipantDto> Participants { get; set; } = new();
    }
}
