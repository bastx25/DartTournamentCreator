using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DTC.Api.Data;
using DTC.Api.Dtos.Auth;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DTC.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DartDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            DartDbContext context,
            IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Ein Benutzer mit dieser E-Mail-Adresse existiert bereits.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName?.Trim(),
                LastName = dto.LastName?.Trim(),
                CreatedAt = DateTimeOffset.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registrierung fehlgeschlagen: {errors}");
            }

            var (accessToken, expiresAt) = GenerateJwtToken(user);
            var refreshToken = GenerateAndSaveRefreshToken(user, ipAddress);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, string? ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Ungültige E-Mail-Adresse oder falsches Passwort.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Ungültige E-Mail-Adresse oder falsches Passwort.");
            }

            var (accessToken, expiresAt) = GenerateJwtToken(user);
            var refreshToken = GenerateAndSaveRefreshToken(user, ipAddress);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException("Refresh Token ist erforderlich.");
            }

            var refreshToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Ungültiger Refresh Token.");
            }

            if (refreshToken.IsRevoked)
            {
                // Replay attack detection: revoke all tokens for this user
                var activeTokens = await _context.RefreshTokens
                    .Where(r => r.UserId == refreshToken.UserId && r.RevokedAt == null)
                    .ToListAsync();

                foreach (var t in activeTokens)
                {
                    t.RevokedAt = DateTimeOffset.UtcNow;
                    t.RevokedByIp = ipAddress;
                }
                await _context.SaveChangesAsync();

                throw new UnauthorizedAccessException("Ungültiger oder bereits widerrufener Refresh Token.");
            }

            if (refreshToken.IsExpired)
            {
                throw new UnauthorizedAccessException("Der Refresh Token ist abgelaufen. Bitte erneut anmelden.");
            }

            var user = refreshToken.User;

            // Rotate refresh token
            var newRefreshToken = GenerateAndSaveRefreshToken(user, ipAddress);
            refreshToken.RevokedAt = DateTimeOffset.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            var (accessToken, expiresAt) = GenerateJwtToken(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = expiresAt,
                User = MapToUserDto(user)
            };
        }

        public async Task RevokeTokenAsync(string token, string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.RevokedAt = DateTimeOffset.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? null : MapToUserDto(user);
        }

        private (string Token, DateTimeOffset ExpiresAt) GenerateJwtToken(ApplicationUser user)
        {
            var jwtKey = _config["Jwt:Key"] ?? "DartTournamentCreatorSuperSecretKeyForJwtAuthentication2026!";
            var issuer = _config["Jwt:Issuer"] ?? "DartTournamentCreator";
            var audience = _config["Jwt:Audience"] ?? "DartTournamentCreatorClient";
            var durationMinutes = _config.GetValue("Jwt:AccessTokenDurationMinutes", 15);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(durationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim("firstName", user.FirstName));
            }
            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim("lastName", user.LastName));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt.UtcDateTime,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), expiresAt);
        }

        private RefreshToken GenerateAndSaveRefreshToken(ApplicationUser user, string? ipAddress)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var tokenString = Convert.ToBase64String(randomBytes);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = tokenString,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedByIp = ipAddress
            };

            _context.RefreshTokens.Add(refreshToken);
            return refreshToken;
        }

        private static UserDto MapToUserDto(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
