using DTC.Api.Dtos.Board;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class BoardMapper
    {
        // Board Mappings
        public static BoardDto ToBoardDto(this Board board)
        {
            return new BoardDto
            {
                Id = board.Id,
                LocationId = board.LocationId,
                Number = board.Number,
                Label = board.Label,
                IsActive = board.IsActive
            };
        }

        public static Board ToEntityFromCreate(this CreateBoardDto dto)
        {
            return new Board
            {
                LocationId = dto.LocationId,
                Number = dto.Number,
                Label = dto.Label?.Trim(),
                IsActive = dto.IsActive
            };
        }

        public static void UpdateEntity(this UpdateBoardDto dto, Board board)
        {
            board.Number = dto.Number;
            board.Label = dto.Label?.Trim();
            board.IsActive = dto.IsActive;
        }
    }
}
