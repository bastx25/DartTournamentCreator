using System.Security.Claims;
using DTC.Api.Dtos.Location;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Authorize]
    [Route("api/locations")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepo;

        public LocationController(ILocationRepository locationRepo)
        {
            _locationRepo = locationRepo;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Benutzer ist nicht authentifiziert.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
        {
            var userId = GetCurrentUserId();
            var locations = await _locationRepo.GetByUserIdAsync(userId);
            return Ok(locations.Select(l => l.ToLocationDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LocationDto>> GetById([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var location = await _locationRepo.GetByIdAndUserIdAsync(id, userId);
            if (location == null)
            {
                return NotFound(new { message = "Austragungsort nicht gefunden oder Zugriff verweigert." });
            }

            return Ok(location.ToLocationDto());
        }

        [HttpPost]
        public async Task<ActionResult<LocationDto>> Create([FromBody] CreateLocationDto dto)
        {
            var userId = GetCurrentUserId();
            var entity = dto.ToEntityFromCreate();
            entity.UserId = userId;

            var created = await _locationRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToLocationDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateLocationDto dto)
        {
            var userId = GetCurrentUserId();
            var existing = await _locationRepo.GetByIdAndUserIdAsync(id, userId);
            if (existing == null)
            {
                return NotFound(new { message = "Austragungsort nicht gefunden oder Zugriff verweigert." });
            }

            dto.UpdateLocationEntity(existing);
            var location = await _locationRepo.UpdateAsync(existing);

            return Ok(location.ToLocationDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var success = await _locationRepo.DeleteAsync(id, userId);
            if (!success)
            {
                return NotFound(new { message = "Austragungsort nicht gefunden oder Zugriff verweigert." });
            }

            return NoContent();
        }
    }
}
