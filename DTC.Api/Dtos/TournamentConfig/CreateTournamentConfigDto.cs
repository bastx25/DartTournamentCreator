using DTC.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.TournamentConfig
{
    public class CreateTournamentConfigDto
    {
        [Required(ErrorMessage = "Die Turnier-ID ist ein Pflichtfeld.")]
        public int TournamentId { get; set; }

        public TournamentMode Mode { get; set; } = TournamentMode.GroupStage;

        [Range(1, int.MaxValue, ErrorMessage = "Die Matchdauer muss mindestens 1 Minute betragen.")]
        public int MatchDurationMinutes { get; set; } = 30;

        [Range(0, int.MaxValue, ErrorMessage = "Die Pause zwischen den Spielen darf nicht negativ sein.")]
        public int BreakBetweenMatchesMinutes { get; set; } = 5;

        [Range(1, int.MaxValue, ErrorMessage = "Die Anzahl der Gruppen muss mindestens 1 betragen.")]
        public int GroupCount { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Die Anzahl der Spieler pro Gruppe muss mindestens 1 betragen.")]
        public int? PlayersPerGroup { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Die Anzahl der Qualifikanten pro Gruppe muss mindestens 1 betragen.")]
        public int QualifiersPerGroup { get; set; } = 1;
    }
}
