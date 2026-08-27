using DTC.Api.Dtos.Match;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class MatchMapper
    {
        public static MatchDto ToMatchDto(this Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                RoundId = match.RoundId,
                BoardId = match.BoardId,
                Status = match.Status,
                ActualStart = match.ActualStart,
                ActualEnd = match.ActualEnd,
                Participants = match.Participants
                    .Select(p => p.ToMatchParticipantDto())
                    .ToList()
            };
        }

        public static Match ToEntityFromCreate(this CreateMatchDto dto)
        {
            return new Match
            {
                RoundId = dto.RoundId,
                BoardId = dto.BoardId,
                Status = dto.Status
            };
        }

        public static void UpdateEntity(this UpdateMatchDto dto, Match match)
        {
            match.BoardId = dto.BoardId;
            match.Status = dto.Status;
            match.ActualStart = dto.ActualStart;
            match.ActualEnd = dto.ActualEnd;
        }
    }
}
