using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("MatchParticipants")]
    public class MatchParticipant
    {
        public int Id { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        public int TournamentPlayerId { get; set; }
        public TournamentPlayer TournamentPlayer { get; set; } = null!;

        public int Score { get; set; }

        public bool IsWinner { get; set; }
    }
}