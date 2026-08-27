namespace DTC.Api.Dtos.Location
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public List<BoardDto> Boards { get; set; } = new();
    }
}
