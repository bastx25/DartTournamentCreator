using DTC.Api.Dtos.Player;

namespace DTC.Api.Dtos.MatchParticipant
{
    public class MatchParticipantDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public PlayerDto? Player { get; set; } // Verschachteltes Player-DTO für das UI
        public int Score { get; set; }
        public bool IsWinner { get; set; }
    }
}
