using DTC.Api.Dtos.MatchParticipant;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/matchparticipants")]
    [ApiController]

    public class MatchParticipantController : ControllerBase
    {
        private readonly IMatchParticipantRepository _participantRepo;

        public MatchParticipantController(IMatchParticipantRepository participantRepo)
        {
            _participantRepo = participantRepo;
        }

        [HttpGet("match/{matchId:int}")]
        public async Task<ActionResult<List<MatchParticipantDto>>> GetByMatchId([FromRoute] int matchId)
        {
            var participants = await _participantRepo.GetByMatchIdAsync(matchId);
            var dtos = participants.Select(p => p.ToMatchParticipantDto());
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MatchParticipantDto>> GetById([FromRoute] int id)
        {
            var participant = await _participantRepo.GetByIdAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            return Ok(participant.ToMatchParticipantDto());
        }

        [HttpPost]
        public async Task<ActionResult<MatchParticipantDto>> Create([FromBody] CreateMatchParticipantDto dto)
        {
            var entity = dto.ToEntityFromCreate();
            var createdParticipant = await _participantRepo.CreateAsync(entity);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdParticipant.Id },
                createdParticipant.ToMatchParticipantDto()
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMatchParticipantDto dto)
        {
            var existingParticipant = await _participantRepo.GetByIdAsync(id);
            if (existingParticipant == null)
            {
                return NotFound();
            }

            dto.UpdateEntity(existingParticipant);
            await _participantRepo.UpdateAsync(existingParticipant);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _participantRepo.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
