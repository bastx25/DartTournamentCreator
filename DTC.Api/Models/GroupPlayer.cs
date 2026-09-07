namespace DTC.Api.Models
{
    public class GroupPlayer
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public int TournamentPlayerId { get; set; }
        public TournamentPlayer TournamentPlayer { get; set; } = null!;
    }
}