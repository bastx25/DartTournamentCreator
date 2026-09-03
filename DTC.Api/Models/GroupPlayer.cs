namespace DTC.Api.Models
{
    public class GroupPlayer
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int PlayerId { get; set; }

        public Group Group { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
