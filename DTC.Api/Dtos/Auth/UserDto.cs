namespace DTC.Api.Dtos.Auth
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string DisplayName => $"{FirstName} {LastName}".Trim().Length > 0
            ? $"{FirstName} {LastName}".Trim()
            : (UserName ?? Email);
    }
}
