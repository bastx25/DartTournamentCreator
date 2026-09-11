using DTC.Api.Dtos.Match;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using DTC.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IMatchMakerService _matchMakerService;

        public MatchController(IMatchRepository matchRepo, IMatchMakerService matchMakerService)
        {
            _matchRepo = matchRepo;
            _matchMakerService = matchMakerService;
        }

        [HttpGet("round/{roundId:int}")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetByBraketId([FromRoute] int roundId)
        {
            var matches = await _matchRepo.GetByBraketIdAsync(roundId);
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

            dto.UpdateMatchEntity(existingMatch);
            await _matchRepo.UpdateAsync(existingMatch);

            if (existingMatch.Status == DTC.Api.Enums.MatchStatus.Completed)
                await _matchMakerService.AdvanceKnockoutAsync(existingMatch.Id);

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
