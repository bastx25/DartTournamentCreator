namespace DTC.Api.Models
{
    public class MatchParticipant
    {
        public int Id { get; set; }
        public int MatchId { get; set; }  // Foreign Key
        public int PlayerId { get; set; } // Foreign Key

        public int Score { get; set; }
        public bool IsWinner { get; set; }

        // Navigation Properties
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
