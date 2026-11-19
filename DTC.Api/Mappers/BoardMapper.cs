using DTC.Api.Dtos.Board;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class BoardMapper
    {
        // Entity -> DTO
        public static BoardDto ToBoardDto(this Board board)
        {
            return new BoardDto
            {
                Id = board.Id,
                LocationId = board.LocationId,
                Label = board.Label,
                IsActive = board.IsActive
            };
        }

        // Create DTO -> Entity
        public static Board ToEntityFromCreate(this CreateBoardDto dto)
        {
            return new Board
            {
                LocationId = dto.LocationId,
                Label = dto.Label?.Trim(),
                IsActive = dto.IsActive
            };
        }

        // Update DTO -> vorhandene Entity aktualisieren
        public static void UpdateBoardEntity(
            this UpdateBoardDto dto,
            Board board)
        {
            board.LocationId = dto.LocationId;
            board.Label = dto.Label?.Trim();
            board.IsActive = dto.IsActive;
        }
    }
}
