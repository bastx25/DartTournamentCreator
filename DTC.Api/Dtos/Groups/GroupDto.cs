using DTC.Api.Dtos.GroupPlayer;
using DTC.Api.Dtos.Round;
using DTC.Api.Models; 

namespace DTC.Api.Dtos.Groups
{
    public class GroupDto
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int BoardId { get; set; }
        public int Sequence { get; set; }
        public string Name { get; set; } = string.Empty;
        public int QualifiersCount { get; set; }

        public ICollection<GroupPlayerDto> Players { get; set; } = new List<GroupPlayerDto>();
        public ICollection<RoundDto> Rounds { get; set; } = new List<RoundDto>();
    }
}
