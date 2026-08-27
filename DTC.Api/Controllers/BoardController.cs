using DTC.Api.Dtos.Board;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/boards")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly IBoardRepository _boardRepo;

        public BoardController(IBoardRepository boardRepo)
        {
            _boardRepo = boardRepo;
        }

        [HttpGet("location/{locationId:int}")]
        public async Task<ActionResult<IEnumerable<BoardDto>>> GetByLocation([FromRoute] int locationId)
        {
            var boards = await _boardRepo.GetByLocationIdAsync(locationId);
            return Ok(boards.Select(b => b.ToBoardDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BoardDto>> GetById([FromRoute] int id)
        {
            var board = await _boardRepo.GetByIdAsync(id);
            if (board == null) return NotFound();

            return Ok(board.ToBoardDto());
        }

        [HttpPost]
        public async Task<ActionResult<BoardDto>> Create([FromBody] CreateBoardDto dto)
        {
            var entity = dto.ToEntityFromCreate();
            var created = await _boardRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToBoardDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBoardDto dto)
        {
            var existing = await _boardRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            dto.UpdateEntity(existing);
            await _boardRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _boardRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
