using DTC.Api.Dtos.Location;

namespace DTC.Api.Dtos.Board
{
    public class BoardDto
    {
        public int Id { get; set; }

        public LocationDto? Location { get; set; }

        public int Number { get; set; }

        public string? Label { get; set; }

        public bool IsActive { get; set; }
    }
}
