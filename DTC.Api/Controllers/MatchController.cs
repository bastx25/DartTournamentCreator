using DTC.Api.Dtos.Match;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository _matchRepo;

        public MatchController(IMatchRepository matchRepo)
        {
            _matchRepo = matchRepo;
        }

        [HttpGet("round/{roundId:int}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetByRoundId([FromRoute] int roundId)
        {
            var matches = await _matchRepo.GetByRoundIdAsync(roundId);
            var dtos = matches.Select(m => m.ToMatchDto());
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MatchDto>> GetById([FromRoute] int id)
        {
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
            var existingMatch = await _matchRepo.GetByIdAsync(id);
            if (existingMatch == null)
            {
                return NotFound();
            }

            dto.UpdateEntity(existingMatch);
            await _matchRepo.UpdateAsync(existingMatch);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _matchRepo.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
