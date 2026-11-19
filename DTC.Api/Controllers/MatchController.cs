using System.Security.Claims;
using DTC.Api.Dtos.Match;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using DTC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IMatchMakerService _matchMakerService;
        private readonly ITournamentAuthorizationService _authService;

        public MatchController(
            IMatchRepository matchRepo,
            IMatchMakerService matchMakerService,
            ITournamentAuthorizationService authService)
        {
            _matchRepo = matchRepo;
            _matchMakerService = matchMakerService;
            _authService = authService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpGet("braket/{braketId:int}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetByBraketId([FromRoute] int braketId)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasBraketAccessAsync(braketId, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Runde nicht gefunden oder Zugriff verweigert." });
            }

            var matches = await _matchRepo.GetByBraketIdAsync(braketId);
            var dtos = matches.Select(m => m.ToMatchDto());
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MatchDto>> GetById([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasMatchAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Match nicht gefunden oder Zugriff verweigert." });
            }

            var match = await _matchRepo.GetByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            return Ok(match.ToMatchDto());
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> Create([FromBody] CreateMatchDto dto)
        {
            var userId = GetCurrentUserId();
            if (dto.BoardId.HasValue)
            {
                var hasBoardAccess = await _authService.HasBoardAccessAsync(dto.BoardId.Value, userId);
                if (!hasBoardAccess)
                {
                    return BadRequest(new { message = "Das ausgewählte Board existiert nicht oder gehört nicht zu deinem Benutzerkonto." });
                }
            }

            var entity = dto.ToEntityFromCreate();
            var createdMatch = await _matchRepo.CreateAsync(entity);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdMatch.Id },
                createdMatch.ToMatchDto()
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMatchDto dto)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasMatchAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Match nicht gefunden oder Zugriff verweigert." });
            }

            if (dto.BoardId.HasValue)
            {
                var hasBoardAccess = await _authService.HasBoardAccessAsync(dto.BoardId.Value, userId);
                if (!hasBoardAccess)
                {
                    return BadRequest(new { message = "Das ausgewählte Board existiert nicht oder gehört nicht zu deinem Benutzerkonto." });
                }
            }

            var existingMatch = await _matchRepo.GetByIdAsync(id);
            if (existingMatch == null)
            {
                return NotFound();
            }

            dto.UpdateMatchEntity(existingMatch);
            await _matchRepo.UpdateAsync(existingMatch);

            if (existingMatch.Status == DTC.Api.Enums.MatchStatus.Completed)
                await _matchMakerService.AdvanceKnockoutAsync(existingMatch.Id);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasMatchAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Match nicht gefunden oder Zugriff verweigert." });
            }

            var success = await _matchRepo.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
