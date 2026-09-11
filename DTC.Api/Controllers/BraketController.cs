using DTC.Api.Dtos.Braket;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/rounds")]
    [ApiController]
    public class BraketController : ControllerBase
    {
        private readonly IBraketRepository _roundRepo;
        private readonly IMatchRepository _matchRepo;

        public BraketController(IBraketRepository roundRepo, IMatchRepository matchRepository)
        {
            _roundRepo = roundRepo;
            _matchRepo = matchRepository;
        }

        [HttpGet("tournament/{tournamentId:int}")]
        public async Task<ActionResult<IEnumerable<BraketDto>>> GetByTournament([FromRoute] int tournamentId)
        {
            var rounds = await _roundRepo.GetByTournamentIdAsync(tournamentId);
            return Ok(rounds.Select(r => r.ToBraketDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BraketDto>> GetById([FromRoute] int id)
        {
            var round = await _roundRepo.GetByIdAsync(id);
            if (round == null) return NotFound();

            return Ok(round.ToBraketDto());
        }

        [HttpPost]
        public async Task<ActionResult<BraketDto>> Create([FromBody] CreateBraketDto dto)
        {
            var entity = dto.ToEntityFromCreate();
            var created = await _roundRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToBraketDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateBraketDto dto)
        {
            var existing = await _roundRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            dto.UpdateBraketEntity(existing);
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

        [HttpGet("{id:int}/matches")]
        public async Task<IActionResult> GetMatchesByBraketId([FromRoute] int id)
        {
            try
            {
                var matches = await _matchRepo.GetByBraketIdAsync(id);
                return Ok(matches.Select(m => m.ToMatchDto()));
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
