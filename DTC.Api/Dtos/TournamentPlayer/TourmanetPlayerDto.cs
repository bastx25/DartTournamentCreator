using DTC.Api.Dtos.Player;

namespace DTC.Api.Dtos.TournamentPlayer
{
    public class TournamentPlayerDto
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public PlayerDto? Player { get; set; }
    }
}
