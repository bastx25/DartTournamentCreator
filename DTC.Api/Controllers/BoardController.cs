using System.Security.Claims;
using DTC.Api.Dtos.Board;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/boards")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly IBoardRepository _boardRepo;
        private readonly ITournamentAuthorizationService _authService;

        public BoardController(
            IBoardRepository boardRepo,
            ITournamentAuthorizationService authService)
        {
            _boardRepo = boardRepo;
            _authService = authService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardDto>>> GetAllAsync()
        {
            var userId = GetCurrentUserId();
            var boards = await _boardRepo.GetByUserIdAsync(userId);
            return Ok(boards.Select(b => b.ToBoardDto()));
        }

        [HttpGet("location/{locationId:int}")]
        public async Task<ActionResult<IEnumerable<BoardDto>>> GetByLocation([FromRoute] int locationId)
        {
            var userId = GetCurrentUserId();
            var hasLocationAccess = await _authService.HasLocationAccessAsync(locationId, userId);
            if (!hasLocationAccess)
            {
                return NotFound(new { message = "Austragungsort nicht gefunden oder Zugriff verweigert." });
            }

            var boards = await _boardRepo.GetByLocationIdAndUserIdAsync(locationId, userId);
            return Ok(boards.Select(b => b.ToBoardDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BoardDto>> GetById([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var board = await _boardRepo.GetByIdAndUserIdAsync(id, userId);
            if (board == null)
            {
                return NotFound(new { message = "Board nicht gefunden oder Zugriff verweigert." });
            }

            return Ok(board.ToBoardDto());
        }

        [HttpPost]
        public async Task<ActionResult<BoardDto>> Create([FromBody] CreateBoardDto dto)
        {
            var userId = GetCurrentUserId();

            // Validate that the target location belongs to the logged-in user
            var hasLocationAccess = await _authService.HasLocationAccessAsync(dto.LocationId, userId);
            if (!hasLocationAccess)
            {
                return BadRequest(new { message = "Der ausgewählte Austragungsort existiert nicht oder gehört nicht zu deinem Benutzerkonto." });
            }

            var entity = dto.ToEntityFromCreate();
            entity.UserId = userId;

            try
            {
                var created = await _boardRepo.CreateAsync(entity);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToBoardDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBoardDto dto)
        {
            var userId = GetCurrentUserId();
            var existing = await _boardRepo.GetByIdAndUserIdAsync(id, userId);
            if (existing == null)
            {
                return NotFound(new { message = "Board nicht gefunden oder Zugriff verweigert." });
            }

            // Validate that the target location belongs to the logged-in user
            var hasLocationAccess = await _authService.HasLocationAccessAsync(dto.LocationId, userId);
            if (!hasLocationAccess)
            {
                return BadRequest(new { message = "Der ausgewählte Austragungsort existiert nicht oder gehört nicht zu deinem Benutzerkonto." });
            }

            dto.UpdateBoardEntity(existing);
            var updated = await _boardRepo.UpdateAsync(existing);

            return Ok(updated.ToBoardDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var success = await _boardRepo.DeleteAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Board nicht gefunden oder Zugriff verweigert." });
            }

            return NoContent();
        }
    }
}
