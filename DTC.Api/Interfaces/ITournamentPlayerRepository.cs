namespace DTC.Api.Interfaces
{
    public interface ITournamentPlayerRepository
    {
        Task SetTournamentPlayers(int id, List<int>? playerIds);
    }
}
