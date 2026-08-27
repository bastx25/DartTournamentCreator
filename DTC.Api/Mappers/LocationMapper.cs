using DTC.Api.Dtos.Board;
using DTC.Api.Dtos.Location;
using DTC.Api.Models;

namespace DTC.Api.Mappers
{
    public static class LocationMapper
    {
        // Location Mappings
        public static LocationDto ToLocationDto(this Location location)
        {
            return new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                Boards = location.Boards.Select(b => b.ToBoardDto()).ToList()
            };
        }

        public static Location ToEntityFromCreate(this CreateLocationDto dto)
        {
            return new Location
            {
                Name = dto.Name.Trim(),
                Address = dto.Address?.Trim()
            };
        }

        public static void UpdateEntity(this UpdateLocationDto dto, Location location)
        {
            location.Name = dto.Name.Trim();
            location.Address = dto.Address?.Trim();
        }
    }
}
