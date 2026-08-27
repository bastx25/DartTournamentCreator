using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.MatchParticipant
{
    public class CreateMatchParticipantDto
    {
        [Required(ErrorMessage = "MatchId ist ein Pflichtfeld.")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "PlayerId ist ein Pflichtfeld.")]
        public int PlayerId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Der Score muss mindestens 0 sein.")]
        public int Score { get; set; } = 0;

        public bool IsWinner { get; set; } = false;
    }
}
