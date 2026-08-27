namespace DTC.Api.Controllers;

using DTC.Api.Dtos.Player;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using DTC.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

[Route("api/players")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly IPlayerRepository _playerRepo;

    public PlayerController(IPlayerRepository playerRepository)
    {
        _playerRepo = playerRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllPlayers()
    {
        var players = await _playerRepo.GetAllAsync();
        var playerDtos = players.Select(p => p.ToPlayerDto());

        return Ok(playerDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerDto>> GetPlayerById(int id)
    {
        var player = await _playerRepo.GetByIdAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        return Ok(player.ToPlayerDto());
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> CreatePlayer([FromBody] CreatePlayerDto playerDto)
    {
        var playerEntity = playerDto.ToEntityFromCreate();
        
        var createdPlayer = await _playerRepo.CreateAsync(playerEntity);

        return CreatedAtAction(
            nameof(GetPlayerById),
            new { id = createdPlayer.Id },
            createdPlayer.ToPlayerDto()
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePlayer([FromRoute] int id, [FromBody] UpdatePlayerDto playerDto)
    {
        var existingPlayer = await _playerRepo.GetByIdAsync(id);
        if (existingPlayer == null)
        {
            return NotFound();
        }

        playerDto.UpdatePlayerModel(existingPlayer);

        await _playerRepo.UpdateAsync(existingPlayer);
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePlayer([FromRoute] int id)
    {
        var success = await _playerRepo.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}