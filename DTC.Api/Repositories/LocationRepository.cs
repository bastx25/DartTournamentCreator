using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DartDbContext _context;

        public LocationRepository(DartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Include(l => l.Boards)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetByUserIdAsync(string userId)
        {
            return await _context.Locations
                .Include(l => l.Boards)
                .Where(l => l.UserId == userId)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations
                .Include(l => l.Boards)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Location?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Locations
                .Include(l => l.Boards)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        }

        public async Task<Location> CreateAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var location = await _context.Locations
                .Include(l => l.Boards)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (location == null) return false;

            if (location.Boards.Any())
            {
                var boardIds = location.Boards.Select(b => b.Id).ToList();
                var matches = await _context.Matches
                    .Where(m => m.BoardId != null && boardIds.Contains(m.BoardId.Value))
                    .ToListAsync();
                foreach (var match in matches)
                {
                    match.BoardId = null;
                }
                _context.Boards.RemoveRange(location.Boards);
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var location = await _context.Locations
                .Include(l => l.Boards)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (location == null) return false;

            if (location.Boards.Any())
            {
                var boardIds = location.Boards.Select(b => b.Id).ToList();
                var matches = await _context.Matches
                    .Where(m => m.BoardId != null && boardIds.Contains(m.BoardId.Value))
                    .ToListAsync();
                foreach (var match in matches)
                {
                    match.BoardId = null;
                }
                _context.Boards.RemoveRange(location.Boards);
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
