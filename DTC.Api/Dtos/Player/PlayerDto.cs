namespace DTC.Api.Dtos.Player
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Nickname { get; set; }

        /// <summary>
        /// Berechneter Name für eine einfache Darstellung im UI (z. B. "Max 'The Power' Mustermann" oder "Max Mustermann").
        /// </summary>
        public string DisplayName => string.IsNullOrWhiteSpace(Nickname)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} '{Nickname}' {LastName}";
    }
}
