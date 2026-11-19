namespace DTC.Api.Interfaces
{
    public interface ITournamentCodeGenerator
    {
        Task<string> GenerateUniqueTournamentCodeAsync();
    }
}
