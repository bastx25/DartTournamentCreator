using DTC.Api.Dtos.Auth;

namespace DTC.Api.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, string? ipAddress);
        Task<AuthResponseDto> RefreshTokenAsync(string token, string? ipAddress);
        Task RevokeTokenAsync(string token, string? ipAddress);
        Task<UserDto?> GetCurrentUserAsync(string userId);
    }
}
