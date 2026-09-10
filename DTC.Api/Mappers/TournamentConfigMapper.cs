using DTC.Api.Dtos.TournamentConfig;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class TournamentConfigMapper
    {
        /// <summary>
        /// Entity -> DTO
        /// </summary>
        public static TournamentConfigDto ToTournamentConfigDto(
            this TournamentConfig config)
        {
            return new TournamentConfigDto
            {
                Id = config.Id,
                TournamentId = config.TournamentId,
                Mode = config.Mode,
                MatchDurationMinutes = config.MatchDurationMinutes,
                BreakBetweenMatchesMinutes = config.BreakBetweenMatchesMinutes,
                GroupCount = config.GroupCount,
                PlayersPerGroup = config.PlayersPerGroup,
                QualifiersPerGroup = config.QualifiersPerGroup
            };
        }


        /// <summary>
        /// Create DTO -> Entity
        /// </summary>
        public static TournamentConfig ToEntityFromCreate(
            this CreateTournamentConfigDto dto)
        {
            return new TournamentConfig
            {
                // Id wird von der Datenbank generiert.
                TournamentId = dto.TournamentId,
                Mode = dto.Mode,
                MatchDurationMinutes = dto.MatchDurationMinutes,
                BreakBetweenMatchesMinutes = dto.BreakBetweenMatchesMinutes,
                GroupCount = dto.GroupCount,
                PlayersPerGroup = dto.PlayersPerGroup,
                QualifiersPerGroup = dto.QualifiersPerGroup
            };
        }


        /// <summary>
        /// Update DTO -> vorhandene Entity aktualisieren
        /// </summary>
        public static void UpdateTournamentConfigEntity(
            this UpdateTournamentConfigDto dto,
            TournamentConfig config)
        {
            config.TournamentId = dto.TournamentId;
            config.Mode = dto.Mode;
            config.MatchDurationMinutes = dto.MatchDurationMinutes;
            config.BreakBetweenMatchesMinutes = dto.BreakBetweenMatchesMinutes;
            config.GroupCount = dto.GroupCount;
            config.PlayersPerGroup = dto.PlayersPerGroup;
            config.QualifiersPerGroup = dto.QualifiersPerGroup;
        }
    }
}
