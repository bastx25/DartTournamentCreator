using DTC.Api.Dtos.Tournament;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class TournamentMapper
    {
        // Tournament Mappings
        public static TournamentDto ToTournamentDto(this Tournament tournament)
        {
            return new TournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                StartDate = tournament.StartDate,
                Description = tournament.Description,
                Mode = tournament.Mode,
                Status = tournament.Status,
                MatchDurationMinutes = tournament.MatchDurationMinutes,
                BreakBetweenMatchesMinutes = tournament.BreakBetweenMatchesMinutes,
                Rounds = tournament.Rounds.Select(r => r.ToRoundDto()).ToList()
            };
        }

        public static Tournament ToEntityFromCreate(this CreateTournamentDto dto)
        {
            return new Tournament
            {
                Name = dto.Name.Trim(),
                StartDate = dto.StartDate,
                Description = dto.Description?.Trim(),
                Mode = dto.Mode,
                Status = dto.Status,
                MatchDurationMinutes = dto.MatchDurationMinutes,
                BreakBetweenMatchesMinutes = dto.BreakBetweenMatchesMinutes
            };
        }

        public static void UpdateEntity(this UpdateTournamentDto dto, Tournament tournament)
        {
            tournament.Name = dto.Name.Trim();
            tournament.StartDate = dto.StartDate;
            tournament.Description = dto.Description?.Trim();
            tournament.Mode = dto.Mode;
            tournament.Status = dto.Status;
            tournament.MatchDurationMinutes = dto.MatchDurationMinutes ?? tournament.MatchDurationMinutes;
            tournament.BreakBetweenMatchesMinutes = dto.BreakBetweenMatchesMinutes ?? tournament.BreakBetweenMatchesMinutes;
        }
    }
}
