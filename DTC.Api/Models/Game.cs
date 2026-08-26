using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace DTC.Api.Models
{
    [Table("Games")]
    public class Game
    {
        public int Id { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<int> WinningPlayersId { get; set; } = new List<int>();
        public List<Player> WinningPlayers { get; set; } = new List<Player>();
        public List<int> Scores { get; set; } = new List<int>();

        public string GetScores()
        {
            string scores = string.Empty;

            foreach(var score in Scores)
            {
                scores += score.ToString() + ":";
            }

            return scores;
        }
    }
}
