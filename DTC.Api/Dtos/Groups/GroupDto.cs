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

        public ICollection<GroupPlayer> Players { get; set; } = new List<GroupPlayer>();
        public ICollection<DTC.Api.Models.Round> Rounds { get; set; } = new List<DTC.Api.Models.Round>();
    }
}
