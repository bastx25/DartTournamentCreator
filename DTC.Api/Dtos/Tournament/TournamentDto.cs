using DTC.Api.Dtos.Group;
using DTC.Api.Dtos.Braket;
using DTC.Api.Dtos.TournamentPlayer;
using DTC.Api.Enums;

namespace DTC.Api.Dtos.Tournament
{
    public class TournamentDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TournamentStatus Status { get; set; }

        public DateTimeOffset StartDate { get; set; }
    }
}
