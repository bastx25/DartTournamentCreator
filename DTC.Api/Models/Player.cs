using System.ComponentModel.DataAnnotations.Schema;

namespace DTC.Api.Models
{
    [Table("Players")]
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
