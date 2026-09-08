using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.TournamentPlayer
{
    public class UpdateTournamentPlayerDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Das Turnier ist ein Pflichtfeld.")]
        public int TournamentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Der Spieler ist ein Pflichtfeld.")]
        public int PlayerId { get; set; }
    }
}
