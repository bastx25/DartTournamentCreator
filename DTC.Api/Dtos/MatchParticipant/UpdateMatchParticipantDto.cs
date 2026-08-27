using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.MatchParticipant
{
    public class UpdateMatchParticipantDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Der Score muss mindestens 0 sein.")]
        public int Score { get; set; }

        public bool IsWinner { get; set; }
    }
}
