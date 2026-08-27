using DTC.Api.Dtos.MatchParticipant;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class MatchParticipantMapper
    {
        public static MatchParticipantDto ToMatchParticipantDto(this MatchParticipant participant)
        {
            return new MatchParticipantDto
            {
                Id = participant.Id,
                MatchId = participant.MatchId,
                PlayerId = participant.PlayerId,
                Player = participant.Player?.ToPlayerDto(),
                Score = participant.Score,
                IsWinner = participant.IsWinner
            };
        }

        public static MatchParticipant ToEntityFromCreate(this CreateMatchParticipantDto dto)
        {
            return new MatchParticipant
            {
                MatchId = dto.MatchId,
                PlayerId = dto.PlayerId,
                Score = dto.Score,
                IsWinner = dto.IsWinner
            };
        }

        // In-Place Update für getrackte Entities
        public static void UpdateEntity(this UpdateMatchParticipantDto dto, MatchParticipant participant)
        {
            participant.Score = dto.Score;
            participant.IsWinner = dto.IsWinner;
        }
    }
}
