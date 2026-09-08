using System.ComponentModel.DataAnnotations;

namespace DTC.Api.Dtos.Group
{
    public class CreateGroupDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Das Turnier ist ein Pflichtfeld.")]
        public int TournamentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Die Reihenfolge muss größer als 0 sein.")]
        public int Sequence { get; set; }

        [Required(ErrorMessage = "Der Gruppenname ist ein Pflichtfeld.")]
        [StringLength(100, ErrorMessage = "Der Gruppenname darf maximal 100 Zeichen lang sein.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Die Anzahl der Qualifikanten muss größer als 0 sein.")]
        public int QualifiersCount { get; set; }
    }
}
