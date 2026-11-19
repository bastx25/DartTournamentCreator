namespace DTC.Api.Dtos.Player
{
    public class PlayerDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string? Nickname { get; set; }

        /// <summary>
        /// Berechneter Name für die Darstellung im UI.
        /// Beispiel: "Max 'The Power' Mustermann", "Max Mustermann", "Max 'The Power'" oder "Max".
        /// </summary>
        public string DisplayName
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(FirstName)) parts.Add(FirstName.Trim());
                if (!string.IsNullOrWhiteSpace(Nickname)) parts.Add($"'{Nickname.Trim()}'");
                if (!string.IsNullOrWhiteSpace(LastName)) parts.Add(LastName.Trim());
                return parts.Count > 0 ? string.Join(" ", parts) : string.Empty;
            }
        }
    }
}
