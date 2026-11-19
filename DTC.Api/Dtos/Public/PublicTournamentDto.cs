using DTC.Api.Dtos.Braket;
using DTC.Api.Dtos.Group;
using DTC.Api.Dtos.TournamentConfig;
using DTC.Api.Enums;

namespace DTC.Api.Dtos.Public
{
    public class PublicTournamentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TournamentStatus Status { get; set; }
        public DateTimeOffset StartDate { get; set; }

        public TournamentConfigDto? Config { get; set; }
        public List<GroupDto> Groups { get; set; } = new();
        public List<BraketDto> Brakets { get; set; } = new();
    }
}
