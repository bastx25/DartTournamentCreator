using DTC.Api.Dtos.Player;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class PlayerMapper
    {
        /// <summary>
        /// Entity -> DTO
        /// </summary>
        public static PlayerDto ToPlayerDto(this Player player)
        {
            return new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Nickname = player.Nickname
            };
        }


        /// <summary>
        /// Create DTO -> Entity
        /// </summary>
        public static Player ToEntityFromCreate(this CreatePlayerDto dto)
        {
            return new Player
            {
                // Id wird von der Datenbank generiert.
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Nickname = dto.Nickname?.Trim()
            };
        }


        /// <summary>
        /// Update DTO -> vorhandene Entity aktualisieren
        /// </summary>
        public static void UpdatePlayerEntity(
            this UpdatePlayerDto dto,
            Player player)
        {
            player.FirstName = dto.FirstName.Trim();
            player.LastName = dto.LastName.Trim();
            player.Nickname = dto.Nickname?.Trim();
        }
    }
}
