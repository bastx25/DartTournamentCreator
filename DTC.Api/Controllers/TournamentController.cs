using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Dtos.Tournament;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using DTC.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentRepository _tournamentRepo;
        private readonly IMatchMakerService _matchMakerService;

        public TournamentController(
            ITournamentRepository tournamentRepo,
            IMatchMakerService matchMakerService)
        {
            _tournamentRepo = tournamentRepo;
            _matchMakerService = matchMakerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetAll()
        {
            var tournaments = await _tournamentRepo.GetAllAsync();
            return Ok(tournaments.Select(t => t.ToTournamentDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TournamentDto>> GetById([FromRoute] int id)
        {
            var tournament = await _tournamentRepo.GetByIdAsync(id);
            if (tournament == null) return NotFound();

            return Ok(tournament.ToTournamentDto());
        }

        [HttpPost]
        public async Task<ActionResult<TournamentDto>> Create([FromBody] CreateTournamentDto dto)
        {
            if (dto.MatchDurationMinutes < 1 || dto.BreakBetweenMatchesMinutes < 0)
                return BadRequest("Matchdauer muss mindestens 1 Minute und die Pause darf nicht negativ sein.");

            var entity = dto.ToEntityFromCreate();
            var created = await _tournamentRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToTournamentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTournamentDto dto)
        {
            var existing = await _tournamentRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (dto.MatchDurationMinutes.HasValue && dto.MatchDurationMinutes.Value < 1)
                return BadRequest("Die Matchdauer muss mindestens 1 Minute betragen.");
            if (dto.BreakBetweenMatchesMinutes.HasValue && dto.BreakBetweenMatchesMinutes.Value < 0)
                return BadRequest("Die Pause darf nicht negativ sein.");

            dto.UpdateEntity(existing);
            await _tournamentRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _tournamentRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpPost("{id:int}/generate-groups")]
        public async Task<IActionResult> GenerateGroups([FromRoute] int id, [FromBody] GenerateGroupsDto dto)
        {
            try
            {
                await _matchMakerService.GenerateGroupsAsync(id, dto);
                return Ok(new { Message = "Gruppenphase und vorbereitete K.-o.-Slots wurden erfolgreich generiert." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:int}/generate-knockout")]
        public async Task<IActionResult> GenerateKnockout([FromRoute] int id)
        {
            try
            {
                await _matchMakerService.GenerateKnockoutAsync(id);
                return Ok(new { Message = "K.-o.-Phase wurde anhand der Match-Siege generiert." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
