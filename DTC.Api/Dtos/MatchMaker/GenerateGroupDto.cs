using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.MatchMaker
{
    public class GenerateGroupsDto
    {
        [Range(1, 64, ErrorMessage = "Die Anzahl der Gruppen muss zwischen 1 und 64 liegen.")]
        public int GroupCount { get; set; } = 1;

        [Range(2, 64, ErrorMessage = "Die Gruppengröße muss zwischen 2 und 64 liegen.")]
        public int GroupSize { get; set; } = 4;

        [Range(1, 64, ErrorMessage = "Die Anzahl der Weiterkommer pro Gruppe muss mindestens 1 sein.")]
        public int QualifiersPerGroup { get; set; } = 1;

        public DateTimeOffset? StartTime { get; set; }

        [Range(1, 240, ErrorMessage = "Die Matchdauer muss zwischen 1 und 240 Minuten liegen.")]
        public int? MatchDurationMinutes { get; set; }

        [Range(0, 120, ErrorMessage = "Die Pause muss zwischen 0 und 120 Minuten liegen.")]
        public int? BreakBetweenMatchesMinutes { get; set; }

        public List<int>? PlayerIds { get; set; }
    }
}
