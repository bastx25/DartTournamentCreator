namespace DTC.Api.Dtos.Board
{
    public class BoardDto
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int Number { get; set; }
        public string? Label { get; set; }
        public bool IsActive { get; set; }
    }
}
