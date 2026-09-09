using DTC.Api.Dtos.Board;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class BoardMatchMapper
    {
        public static BoardMatchDto ToBoardMatchDto(this Match match)
        {
            return new BoardMatchDto
            {
                MatchId = match.Id,
                RoundId = match.RoundId ?? -1,
                RoundName = match.Round?.Name,
                TournamentName = match.Round.Tournament?.Name ?? string.Empty,
                PlannedStart = match.PlannedStart,
                ActualStart = match.ActualStart,
                Status = match.Status,
                Participants = match.Participants
                    .OrderBy(p => p.Id)
                    .Select(p => p.ToMatchParticipantDto())
                    .ToList()
            };
        }
    }
}
