using DTC.Api.Dtos.Groups;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class GroupMapper
    {

        public static GroupDto ToGroupDto( this Group group)
        {
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Players = group.Players,
                QualifiersCount = group.QualifiersCount,
                Rounds = group.Rounds.Select(r => r.ToRoundDto()).ToList(),
                Sequence = group.Sequence,
                TournamentId = group.TournamentId,
            };
        }
    }
}
