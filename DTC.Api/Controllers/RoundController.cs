using DTC.Api.Dtos.Round;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/rounds")]
    [ApiController]
    public class RoundController : ControllerBase
    {
        private readonly IRoundRepository _roundRepo;

        public RoundController(IRoundRepository roundRepo)
        {
            _roundRepo = roundRepo;
        }

        [HttpGet("tournament/{tournamentId:int}")]
        public async Task<ActionResult<IEnumerable<RoundDto>>> GetByTournament([FromRoute] int tournamentId)
        {
            var rounds = await _roundRepo.GetByTournamentIdAsync(tournamentId);
            return Ok(rounds.Select(r => r.ToRoundDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoundDto>> GetById([FromRoute] int id)
        {
            var round = await _roundRepo.GetByIdAsync(id);
            if (round == null) return NotFound();

            return Ok(round.ToRoundDto());
        }

        [HttpPost]
        public async Task<ActionResult<RoundDto>> Create([FromBody] CreateRoundDto dto)
        {
            var entity = dto.ToEntityFromCreate();
            var created = await _roundRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToRoundDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRoundDto dto)
        {
            var existing = await _roundRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            dto.UpdateEntity(existing);
            await _roundRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _roundRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
