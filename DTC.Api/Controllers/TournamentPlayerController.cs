using DTC.Api.Dtos.Player;
using DTC.Api.Dtos.TournamentPlayer;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/tournamentplayers")]
    [ApiController]
    public class TournamentPlayerController : ControllerBase
    {
        private readonly ITournamentPlayerRepository _tPlayerRepo;

        public TournamentPlayerController(ITournamentPlayerRepository tournamentPlayerRepository)
        {
            _tPlayerRepo = tournamentPlayerRepository;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePlayer([FromRoute] int id, [FromBody] UpdateTournamentPlayerDto playerDto)
        {
            var existingPlayer = await _tPlayerRepo.GetByIdAsync(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            playerDto.UpdateTournamentPlayerEntity(existingPlayer);

            var updatedPlayer = await _tPlayerRepo.UpdateAsync(existingPlayer);

            return Ok(updatedPlayer.ToTournamentPlayerDto());
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePlayer([FromBody] TournamentPlayerDto[] playerDto)
        {
            foreach(var player in playerDto)
            {
                var existingPlayer = await _tPlayerRepo.GetByIdAsync(player.Id);
                if (existingPlayer == null)
                {
                    return NotFound();
                }

                existingPlayer.Id = player.Id;
                existingPlayer.TournamentId = player.TournamentId;
                existingPlayer.IsQualified = player.IsQualified;
                existingPlayer.PlayerId = player.PlayerId;

        var updatedPlayer = await _tPlayerRepo.UpdateAsync(existingPlayer);
            }

            return Ok();
        }

    }
}
