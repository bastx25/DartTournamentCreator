using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.MatchParticipant
{
    public class CreateMatchParticipantDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Das Match ist ein Pflichtfeld.")]
        public int MatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Der Turnierspieler ist ein Pflichtfeld.")]
        public int TournamentPlayerId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Der Score darf nicht negativ sein.")]
        public int Score { get; set; }

        public bool IsWinner { get; set; }
    }
}
