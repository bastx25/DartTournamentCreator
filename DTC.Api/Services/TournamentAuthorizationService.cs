using DTC.Api.Data;
using DTC.Api.Interfaces;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Services
{
    public class TournamentAuthorizationService : ITournamentAuthorizationService
    {
        private readonly DartDbContext _context;

        public TournamentAuthorizationService(DartDbContext context)
        {
            _context = context;
        }

        public async Task<Tournament?> GetAuthorizedTournamentAsync(int tournamentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _context.Tournaments
                .Include(t => t.Config)
                .FirstOrDefaultAsync(t => t.Id == tournamentId && t.UserId == userId);
        }

        public async Task<bool> HasTournamentAccessAsync(int tournamentId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.Tournaments
                .AnyAsync(t => t.Id == tournamentId && t.UserId == userId);
        }

        public async Task<bool> HasGroupAccessAsync(int groupId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.Groups
                .AnyAsync(g => g.Id == groupId && g.Tournament.UserId == userId);
        }

        public async Task<bool> HasBraketAccessAsync(int braketId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.Brakets
                .AnyAsync(b => b.Id == braketId && b.Tournament.UserId == userId);
        }

        public async Task<bool> HasMatchAccessAsync(int matchId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.Matches
                .AnyAsync(m => m.Id == matchId && (
                    (m.Group != null && m.Group.Tournament.UserId == userId) ||
                    (m.Braket != null && m.Braket.Tournament.UserId == userId)));
        }

        public async Task<bool> HasTournamentPlayerAccessAsync(int tournamentPlayerId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            return await _context.TournamentPlayers
                .AnyAsync(tp => tp.Id == tournamentPlayerId && tp.Tournament.UserId == userId);
        }
    }
}
