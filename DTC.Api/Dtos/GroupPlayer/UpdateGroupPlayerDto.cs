using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.GroupPlayer
{
    public class UpdateGroupPlayerDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Die Gruppe ist ein Pflichtfeld.")]
        public int GroupId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Der Turnierspieler ist ein Pflichtfeld.")]
        public int TournamentPlayerId { get; set; }
    }

}
