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
                GroupId = match.GroupId,
                BoardId = match.BoardId,
                Status = match.Status,
                PlannedStart = match.PlannedStart,
                PlannedEnd = match.PlannedEnd,
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
                GroupId = dto.GroupId,
                BoardId = dto.BoardId,
                Status = dto.Status,
                PlannedStart = dto.PlannedStart,
                PlannedEnd = dto.PlannedEnd
            };
        }

        public static void UpdateEntity(this UpdateMatchDto dto, Match match)
        {
            match.BoardId = dto.BoardId;
            match.Status = dto.Status;
            match.PlannedStart = dto.PlannedStart;
            match.PlannedEnd = dto.PlannedEnd;
            match.ActualStart = dto.ActualStart;
            match.ActualEnd = dto.ActualEnd;
        }
    }
}
