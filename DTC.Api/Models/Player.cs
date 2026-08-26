namespace DTC.Api.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Nickname { get; set; }

        // Navigation Properties
        public ICollection<MatchParticipant> MatchParticipants { get; set; } = new List<MatchParticipant>();
    }
}
