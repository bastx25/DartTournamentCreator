using DTC.Api.Enums;

namespace DTC.Api.Dtos.Tournament
{
    public class TournamentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public string? Description { get; set; }
        public TournamentMode Mode { get; set; }
        public TournamentStatus Status { get; set; }
        public List<RoundDto> Rounds { get; set; } = new();
    }
}
