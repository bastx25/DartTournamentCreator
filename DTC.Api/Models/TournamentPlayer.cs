namespace DTC.Api.Models
{
    public class TournamentPlayer
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        // Navigation Properties
        public ICollection<GroupPlayer> Groups { get; set; }
            = new List<GroupPlayer>();

        public ICollection<MatchParticipant> MatchParticipants { get; set; }
            = new List<MatchParticipant>();
    }
}