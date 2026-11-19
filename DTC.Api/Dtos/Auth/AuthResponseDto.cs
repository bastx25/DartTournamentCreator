namespace DTC.Api.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
