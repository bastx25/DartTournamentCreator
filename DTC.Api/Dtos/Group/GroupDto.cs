using DTC.Api.Dtos.GroupPlayer;
using DTC.Api.Dtos.Match;

namespace DTC.Api.Dtos.Group
{
    public class GroupDto
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public int Sequence { get; set; }

        public string Name { get; set; } = string.Empty;

        public int QualifiersCount { get; set; }

        public ICollection<GroupPlayerDto> GropuPlayers { get; set; } = new List<GroupPlayerDto>();
        public ICollection<MatchDto> Matches { get; set; } = new List<MatchDto>();
    }
}
