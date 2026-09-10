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
        private readonly IGroupRepository _groupRepo;
        private readonly IRoundRepository _roundRepo;
        private readonly ITournamentConfigRepository _configRepo;
        private readonly ITournamentPlayerRepository _tplayerRepo;

        public TournamentController(
            ITournamentRepository tournamentRepo,
            IGroupRepository groupRepository,
            IRoundRepository roundRepository,
            ITournamentConfigRepository tournamentConfigRepository,
            ITournamentPlayerRepository tplayerRepository,
            IMatchMakerService matchMakerService)
        {
            _tournamentRepo = tournamentRepo;
            _matchMakerService = matchMakerService;
            _groupRepo = groupRepository;
            _roundRepo = roundRepository;
            _configRepo = tournamentConfigRepository;
            _tplayerRepo = tplayerRepository;
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
            var entity = dto.ToEntityFromCreate();
            var created = await _tournamentRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToTournamentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTournamentDto dto)
        {
            var existing = await _tournamentRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            dto.UpdateTournamentEntity(existing);
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
                return Ok(new { Message = "Gruppenphase wurde generiert." });
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
        public async Task<IActionResult> GenerateKnockout([FromRoute] int id, [FromBody] GenerateGroupsDto dto)
        {
            try
            {
                await _matchMakerService.GenerateKnockoutAsync(id, dto);
                return Ok(new { Message = "K.-o.-Phase wurde aus den abgeschlossenen Gruppen-/Ausspielrunden generiert." });
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

        [HttpGet("{id:int}/groups")]
        public async Task<IActionResult> GetGroups([FromRoute] int id)
        {
            try
            {
                var groups = await _groupRepo.GetGroupsAsync(id);
                return Ok(groups.Select(g => g.ToGroupDto()));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}/rounds")]
        public async Task<IActionResult> GetRounds([FromRoute] int id)
        {
            try
            {
                var rounds = await _roundRepo.GetRoundsAsync(id);
                return Ok(rounds.Select(r => r.ToRoundDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}/configs")]
        public async Task<IActionResult> GetConfigs([FromRoute] int id)
        {
            try
            {
                var configs = await _configRepo.GetConfigsAsync(tournamentid: id);
                return Ok(configs.Select(c => c.ToTournamentConfigDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}/players")]
        public async Task<IActionResult> GetTournamentPlayers([FromRoute] int id)
        {
            try
            {
                var players = await _tplayerRepo.GetPlayersAsync(tournamentid: id);
                return Ok(players.Select(tp => tp.ToTournamentPlayerDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
