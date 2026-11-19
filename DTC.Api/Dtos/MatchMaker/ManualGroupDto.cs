using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.MatchMaker
{
    public class ManualGroupItemDto
    {
        [Required(ErrorMessage = "Gruppenname ist erforderlich.")]
        [StringLength(50, ErrorMessage = "Gruppenname darf maximal 50 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        public List<int> PlayerIds { get; set; } = new();

        [Range(1, 64, ErrorMessage = "QualifiersCount muss mindestens 1 sein.")]
        public int QualifiersCount { get; set; } = 2;
    }

    public class ManualGroupSetupDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Es muss mindestens eine Gruppe vorhanden sein.")]
        public List<ManualGroupItemDto> Groups { get; set; } = new();

        public DateTimeOffset? StartTime { get; set; }

        [Range(1, 240, ErrorMessage = "Die Matchdauer muss zwischen 1 und 240 Minuten liegen.")]
        public int? MatchDurationMinutes { get; set; }

        [Range(0, 120, ErrorMessage = "Die Pause muss zwischen 0 und 120 Minuten liegen.")]
        public int? BreakBetweenMatchesMinutes { get; set; }

        public KnockoutDrawMode KnockoutDrawMode { get; set; } = KnockoutDrawMode.Random;
    }
}
