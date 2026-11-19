using System.Security.Claims;
using DTC.Api.Dtos.Braket;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/brakets")]
    [ApiController]
    public class BraketController : ControllerBase
    {
        private readonly IBraketRepository _braketRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly ITournamentAuthorizationService _authService;

        public BraketController(
            IBraketRepository braketRepo,
            IMatchRepository matchRepository,
            ITournamentAuthorizationService authService)
        {
            _braketRepo = braketRepo;
            _matchRepo = matchRepository;
            _authService = authService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpGet("tournament/{tournamentId:int}")]
        public async Task<ActionResult<IEnumerable<BraketDto>>> GetByTournament([FromRoute] int tournamentId)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(tournamentId, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            var brakets = await _braketRepo.GetByTournamentIdAsync(tournamentId);
            return Ok(brakets.Select(r => r.ToBraketDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BraketDto>> GetById([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasBraketAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Bracket nicht gefunden oder Zugriff verweigert." });
            }

            var braket = await _braketRepo.GetByIdAsync(id);
            if (braket == null) return NotFound();

            return Ok(braket.ToBraketDto());
        }

        [HttpPost]
        public async Task<ActionResult<BraketDto>> Create([FromBody] CreateBraketDto dto)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(dto.TournamentId, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            var entity = dto.ToEntityFromCreate();
            var created = await _braketRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToBraketDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBraketDto dto)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasBraketAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Bracket nicht gefunden oder Zugriff verweigert." });
            }

            var existing = await _braketRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            dto.UpdateBraketEntity(existing);
            await _braketRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasBraketAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Bracket nicht gefunden oder Zugriff verweigert." });
            }

            var success = await _braketRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/matches")]
        public async Task<IActionResult> GetMatchesByBraketId([FromRoute] int id)
        {
            try
            {
                var matches = await _matchRepo.GetByBraketIdAsync(id);
                return Ok(matches.Select(m => m.ToMatchDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
