using DTC.Api.Dtos.Match;
using DTC.Api.Dtos.Public;
using DTC.Api.Dtos.Tournament;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/public/tournaments")]
    [ApiController]
    [AllowAnonymous]
    public class PublicTournamentController : ControllerBase
    {
        private readonly IPublicTournamentService _publicTournamentService;
        private readonly IMatchRepository _matchRepo;

        public PublicTournamentController(
            IPublicTournamentService publicTournamentService,
            IMatchRepository matchRepo)
        {
            _publicTournamentService = publicTournamentService;
            _matchRepo = matchRepo;
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<PublicTournamentDto>> GetByCode([FromRoute] string code)
        {
            var tournament = await _publicTournamentService.GetTournamentByCodeAsync(code);
            if (tournament == null)
            {
                return NotFound(new { message = $"Kein Turnier mit dem Code '{code}' gefunden." });
            }

            return Ok(tournament);
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<ActionResult<PublicTournamentDto>> GetById([FromRoute] int id)
        {
            var tournament = await _publicTournamentService.GetTournamentByIdAsync(id);
            if (tournament == null)
            {
                return NotFound(new { message = $"Turnier mit ID {id} nicht gefunden." });
            }

            return Ok(tournament);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetActive()
        {
            var tournaments = await _publicTournamentService.GetActivePublicTournamentsAsync();
            return Ok(tournaments);
        }

        [HttpGet("{code}/groups/{groupId:int}/matches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetGroupMatches(
            [FromRoute] string code,
            [FromRoute] int groupId)
        {
            var tournament = await _publicTournamentService.GetTournamentByCodeAsync(code);
            if (tournament == null || !tournament.Groups.Any(g => g.Id == groupId))
            {
                return NotFound(new { message = "Gruppe oder Turnier nicht gefunden." });
            }

            var matches = await _matchRepo.GetByGroupIdAsync(groupId);
            return Ok(matches.Select(m => m.ToMatchDto()));
        }

        [HttpGet("{code}/brakets/{braketId:int}/matches")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetBraketMatches(
            [FromRoute] string code,
            [FromRoute] int braketId)
        {
            var tournament = await _publicTournamentService.GetTournamentByCodeAsync(code);
            if (tournament == null || !tournament.Brakets.Any(b => b.Id == braketId))
            {
                return NotFound(new { message = "Bracket oder Turnier nicht gefunden." });
            }

            var matches = await _matchRepo.GetByBraketIdAsync(braketId);
            return Ok(matches.Select(m => m.ToMatchDto()));
        }
    }
}
