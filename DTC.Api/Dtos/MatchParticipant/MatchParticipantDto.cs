using DTC.Api.Dtos.TournamentPlayer;
using DTC.Api.Models;

namespace DTC.Api.Dtos.MatchParticipant
{
    public class MatchParticipantDto
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public TournamentPlayerDto? TournamentPlayer { get; set; }

        public int Score { get; set; }

        public bool IsWinner { get; set; }
    }
}
