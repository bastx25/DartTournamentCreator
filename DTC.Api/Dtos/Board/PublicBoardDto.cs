namespace DTC.Api.Dtos.Board
{
    public class PublicBoardDto
    {
        public int Id { get; set; }
        public string? Label { get; set; }
        public int LocationId { get; set; }
        public string? LocationName { get; set; }
        public bool IsActive { get; set; }
    }
}
