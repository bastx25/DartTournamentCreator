using System.Security.Claims;
using DTC.Api.Dtos.Player;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/players")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepo;
        private readonly ITournamentAuthorizationService _authService;

        public PlayerController(
            IPlayerRepository playerRepository,
            ITournamentAuthorizationService authService)
        {
            _playerRepo = playerRepository;
            _authService = authService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllPlayers()
        {
            var userId = GetCurrentUserId();
            var players = await _playerRepo.GetByUserIdAsync(userId);
            var playerDtos = players.Select(p => p.ToPlayerDto());

            return Ok(playerDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlayerDto>> GetPlayerById(int id)
        {
            var userId = GetCurrentUserId();
            var player = await _playerRepo.GetByIdAndUserIdAsync(id, userId);
            if (player == null)
            {
                return NotFound(new { message = "Spieler nicht gefunden oder Zugriff verweigert." });
            }

            return Ok(player.ToPlayerDto());
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> CreatePlayer([FromBody] CreatePlayerDto playerDto)
        {
            var userId = GetCurrentUserId();
            var playerEntity = playerDto.ToEntityFromCreate();
            playerEntity.UserId = userId;

            var createdPlayer = await _playerRepo.CreateAsync(playerEntity);
            return Ok(createdPlayer.ToPlayerDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePlayer([FromRoute] int id, [FromBody] UpdatePlayerDto playerDto)
        {
            var userId = GetCurrentUserId();
            var existingPlayer = await _playerRepo.GetByIdAndUserIdAsync(id, userId);
            if (existingPlayer == null)
            {
                return NotFound(new { message = "Spieler nicht gefunden oder Zugriff verweigert." });
            }

            playerDto.UpdatePlayerEntity(existingPlayer);
            var updatedPlayer = await _playerRepo.UpdateAsync(existingPlayer);

            return Ok(updatedPlayer.ToPlayerDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlayer([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var success = await _playerRepo.DeleteAsync(id, userId);
            if (!success)
            {
                return BadRequest("Spieler konnte nicht gelöscht werden! Ist der Spieler noch in einem Turnier?");
            }

            return NoContent();
        }
    }
}
