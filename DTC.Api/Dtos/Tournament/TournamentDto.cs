using DTC.Api.Dtos.Groups;
using DTC.Api.Dtos.Round;
using DTC.Api.Dtos.TournamentPlayer;

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
        public int MatchDurationMinutes { get; set; }
        public int BreakBetweenMatchesMinutes { get; set; }
        public List<RoundDto> KnockoutRounds { get; set; } = new();
        public List<GroupDto> Groups { get; set; } = new();
        public List<TournamentPlayerDto> TournamentPlayers { get; set; } = new();
    }
}
