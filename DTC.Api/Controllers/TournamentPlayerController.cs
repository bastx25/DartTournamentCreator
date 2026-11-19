using System.Security.Claims;
using DTC.Api.Dtos.TournamentPlayer;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/tournamentplayers")]
    [ApiController]
    public class TournamentPlayerController : ControllerBase
    {
        private readonly ITournamentPlayerRepository _tPlayerRepo;
        private readonly ITournamentAuthorizationService _authService;

        public TournamentPlayerController(
            ITournamentPlayerRepository tournamentPlayerRepository,
            ITournamentAuthorizationService authService)
        {
            _tPlayerRepo = tournamentPlayerRepository;
            _authService = authService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePlayer([FromRoute] int id, [FromBody] UpdateTournamentPlayerDto playerDto)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentPlayerAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnierspieler nicht gefunden oder Zugriff verweigert." });
            }

            var existingPlayer = await _tPlayerRepo.GetByIdAsync(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            playerDto.UpdateTournamentPlayerEntity(existingPlayer);
            var updatedPlayer = await _tPlayerRepo.UpdateAsync(existingPlayer);

            return Ok(updatedPlayer.ToTournamentPlayerDto());
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePlayer([FromBody] TournamentPlayerDto[] playerDto)
        {
            var userId = GetCurrentUserId();

            foreach (var player in playerDto)
            {
                var hasAccess = await _authService.HasTournamentPlayerAccessAsync(player.Id, userId);
                if (!hasAccess)
                {
                    return NotFound(new { message = $"Zugriff auf Turnierspieler {player.Id} verweigert." });
                }

                var existingPlayer = await _tPlayerRepo.GetByIdAsync(player.Id);
                if (existingPlayer == null)
                {
                    return NotFound();
                }

                existingPlayer.Id = player.Id;
                existingPlayer.TournamentId = player.TournamentId;
                existingPlayer.IsQualified = player.IsQualified;
                existingPlayer.PlayerId = player.PlayerId;

                await _tPlayerRepo.UpdateAsync(existingPlayer);
            }

            return Ok();
        }
    }
}
