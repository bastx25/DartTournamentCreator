using DTC.Api.Dtos.Location;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DTC.Api.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepo;

        public LocationController(ILocationRepository locationRepo)
        {
            _locationRepo = locationRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
        {
            var locations = await _locationRepo.GetAllAsync();
            return Ok(locations.Select(l => l.ToLocationDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LocationDto>> GetById([FromRoute] int id)
        {
            var location = await _locationRepo.GetByIdAsync(id);
            if (location == null) return NotFound();

            return Ok(location.ToLocationDto());
        }

        [HttpPost]
        public async Task<ActionResult<LocationDto>> Create([FromBody] CreateLocationDto dto)
        {
            var entity = dto.ToEntityFromCreate();
            var created = await _locationRepo.CreateAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToLocationDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateLocationDto dto)
        {
            var existing = await _locationRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            dto.UpdateEntity(existing);
            await _locationRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _locationRepo.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
