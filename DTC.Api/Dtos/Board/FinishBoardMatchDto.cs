using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Board
{
    public class FinishBoardMatchDto
    {
        [Required]
        public List<BoardParticipantScoreDto> Participants { get; set; } = new();
    }

    public class BoardParticipantScoreDto
    {
        public int ParticipantId { get; set; }

        [Range(0, int.MaxValue)]
        public int Score { get; set; }
    }
}
