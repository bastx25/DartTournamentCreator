using System.Security.Claims;
using DTC.Api.Dtos.Auth;
using DTC.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private const string RefreshTokenCookieName = "refreshToken";

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var ip = GetIpAddress();
                var result = await _authService.RegisterAsync(dto, ip);
                SetRefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            try
            {
                var ip = GetIpAddress();
                var result = await _authService.LoginAsync(dto, ip);
                SetRefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenRequestDto? dto)
        {
            try
            {
                // Prefer cookie, fallback to request body
                var token = Request.Cookies[RefreshTokenCookieName] ?? dto?.RefreshToken;

                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized(new { message = "Refresh Token fehlt." });
                }

                var ip = GetIpAddress();
                var result = await _authService.RefreshTokenAsync(token, ip);
                SetRefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto? dto)
        {
            var token = Request.Cookies[RefreshTokenCookieName] ?? dto?.RefreshToken;
            if (!string.IsNullOrWhiteSpace(token))
            {
                var ip = GetIpAddress();
                await _authService.RevokeTokenAsync(token, ip);
            }

            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth"
            });

            return Ok(new { message = "Erfolgreich abgemeldet." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _authService.GetCurrentUserAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Benutzer wurde nicht gefunden." });
            }

            return Ok(user);
        }

        private void SetRefreshTokenCookie(string? token)
        {
            if (string.IsNullOrEmpty(token)) return;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth",
                Secure = Request.IsHttps
            };

            Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
        }

        private string? GetIpAddress()
        {
            if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                return forwarded.ToString().Split(',').FirstOrDefault()?.Trim();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
