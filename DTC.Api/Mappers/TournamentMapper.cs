using DTC.Api.Dtos.Tournament;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class TournamentMapper
    {
        /// <summary>
        /// Entity -> DTO
        /// </summary>
        public static TournamentDto ToTournamentDto(
            this Tournament tournament)
        {
            return new TournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Description = tournament.Description,
                Status = tournament.Status,
                StartDate = tournament.StartDate
            };
        }


        /// <summary>
        /// Create DTO -> Entity
        /// </summary>
        public static Tournament ToEntityFromCreate(
            this CreateTournamentDto dto)
        {
            return new Tournament
            {
                // Id wird von der Datenbank generiert.
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Status = dto.Status,
                StartDate = dto.StartDate
            };
        }


        /// <summary>
        /// Update DTO -> vorhandene Entity aktualisieren
        /// </summary>
        public static void UpdateTournamentEntity(
            this UpdateTournamentDto dto,
            Tournament tournament)
        {
            tournament.Name = dto.Name.Trim();
            tournament.Description = dto.Description?.Trim();
            tournament.Status = dto.Status;
            tournament.StartDate = dto.StartDate;
        }
    }
}
