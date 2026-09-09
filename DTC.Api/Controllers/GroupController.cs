using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMatchRepository _matchRepo;

        public GroupController(IMatchRepository matchRepository)
        {
            _matchRepo = matchRepository;
        }

        [HttpGet("{id:int}/matches")]
        public async Task<IActionResult> GetMatchesByRoundId([FromRoute] int id)
        {
            try
            {
                var matches = await _matchRepo.GetByGroupIdAsync(id);
                return Ok(matches.Select(m => m.ToMatchDto()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
