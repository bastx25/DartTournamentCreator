using DTC.Api.Dtos.TournamentConfig;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace DTC.Api.Models
{
    [Table("Availabilities")]
    public class Availability
    {
        public int Id { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public int TournamentPlayerId { get; set; }
        public TournamentPlayer TournamentPlayer { get; set; } = null!;

    }
}
