namespace DTC.Api.Dtos.MatchMaker
{
    public class GenerateGroupsDto
    {
        public int GroupSize { get; set; } = 4; // z. B. 4 Spieler pro Gruppe
        public List<int>? PlayerIds { get; set; } // Optional: Nur bestimmte Spieler mischen
    }
}
