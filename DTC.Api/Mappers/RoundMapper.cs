using DTC.Api.Dtos.Round;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class RoundMapper
    {
        // Round Mappings
        public static RoundDto ToRoundDto(this Round round)
        {
            return new RoundDto
            {
                Id = round.Id,
                TournamentId = round.TournamentId,
                LocationId = round.LocationId,
                Sequence = round.Sequence,
                Name = round.Name,
                PlannedStart = round.PlannedStart,
                PlannedEnd = round.PlannedEnd,
                Status = round.Status,
                Matches = round.Matches.Select(m => m.ToMatchDto()).ToList()
            };
        }

        public static Round ToEntityFromCreate(this CreateRoundDto dto)
        {
            return new Round
            {
                TournamentId = dto.TournamentId,
                LocationId = dto.LocationId,
                Sequence = dto.Sequence,
                Name = dto.Name?.Trim(),
                PlannedStart = dto.PlannedStart,
                PlannedEnd = dto.PlannedEnd,
                Status = dto.Status
            };
        }

        public static void UpdateEntity(this UpdateRoundDto dto, Round round)
        {
            round.LocationId = dto.LocationId;
            round.Sequence = dto.Sequence;
            round.Name = dto.Name?.Trim();
            round.PlannedStart = dto.PlannedStart;
            round.PlannedEnd = dto.PlannedEnd;
            round.Status = dto.Status;
        }
    }
}
