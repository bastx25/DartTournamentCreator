using System.Security.Claims;
using DTC.Api.Dtos.MatchMaker;
using DTC.Api.Dtos.Tournament;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using DTC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentRepository _tournamentRepo;
        private readonly IMatchMakerService _matchMakerService;
        private readonly IGroupRepository _groupRepo;
        private readonly IBraketRepository _braketRepo;
        private readonly ITournamentConfigRepository _configRepo;
        private readonly ITournamentPlayerRepository _tplayerRepo;
        private readonly ITournamentAuthorizationService _authService;
        private readonly ITournamentCodeGenerator _codeGenerator;

        public TournamentController(
            ITournamentRepository tournamentRepo,
            IGroupRepository groupRepository,
            IBraketRepository braketRepository,
            ITournamentConfigRepository tournamentConfigRepository,
            ITournamentPlayerRepository tplayerRepository,
            IMatchMakerService matchMakerService,
            ITournamentAuthorizationService authService,
            ITournamentCodeGenerator codeGenerator)
        {
            _tournamentRepo = tournamentRepo;
            _matchMakerService = matchMakerService;
            _groupRepo = groupRepository;
            _braketRepo = braketRepository;
            _configRepo = tournamentConfigRepository;
            _tplayerRepo = tplayerRepository;
            _authService = authService;
            _codeGenerator = codeGenerator;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetAll()
        {
            var userId = GetCurrentUserId();
            var tournaments = await _tournamentRepo.GetByUserIdAsync(userId);
            return Ok(tournaments.Select(t => t.ToTournamentDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TournamentDto>> GetById([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var tournament = await _authService.GetAuthorizedTournamentAsync(id, userId);
            if (tournament == null)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            return Ok(tournament.ToTournamentDto());
        }

        [HttpPost]
        public async Task<ActionResult<TournamentDto>> Create([FromBody] CreateTournamentDto dto)
        {
            var userId = GetCurrentUserId();
            var entity = dto.ToEntityFromCreate();

            entity.UserId = userId;
            entity.Code = await _codeGenerator.GenerateUniqueTournamentCodeAsync();

            var created = await _tournamentRepo.CreateAsync(entity);
            await _configRepo.CreateNewTConfig(tournamentId: created.Id, dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToTournamentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTournamentDto dto)
        {
            var userId = GetCurrentUserId();
            var existing = await _authService.GetAuthorizedTournamentAsync(id, userId);
            if (existing == null)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            dto.UpdateTournamentEntity(existing);
            await _tournamentRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            var success = await _tournamentRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpPost("{id:int}/generate-groups")]
        public async Task<IActionResult> GenerateGroups([FromRoute] int id, [FromBody] GenerateGroupsDto dto)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            try
            {
                await _configRepo.UpdateConfig(id, dto);
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

        [HttpPost("{id:int}/manual-groups")]
        public async Task<IActionResult> SaveManualGroups([FromRoute] int id, [FromBody] ManualGroupSetupDto dto)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            try
            {
                await _matchMakerService.SaveManualGroupsAsync(id, dto);
                return Ok(new { Message = "Manuelle Gruppen und Matches wurden erfolgreich generiert." });
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
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            try
            {
                await _configRepo.UpdateConfig(id, dto);
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
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            try
            {
                var groups = await _groupRepo.GetGroupsAsync(id);
                return Ok(groups.Select(g => g.ToGroupDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}/brakets")]
        public async Task<IActionResult> GetBrakets([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

            try
            {
                var brakets = await _braketRepo.GetBraketsAsync(id);
                return Ok(brakets.Select(r => r.ToBraketDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}/configs")]
        public async Task<IActionResult> GetConfigs([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

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
            var userId = GetCurrentUserId();
            var hasAccess = await _authService.HasTournamentAccessAsync(id, userId);
            if (!hasAccess)
            {
                return NotFound(new { message = "Turnier nicht gefunden oder Zugriff verweigert." });
            }

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
