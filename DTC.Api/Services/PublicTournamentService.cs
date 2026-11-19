using DTC.Api.Data;
using DTC.Api.Dtos.Public;
using DTC.Api.Dtos.Tournament;
using DTC.Api.Enums;
using DTC.Api.Interfaces;
using DTC.Api.Mappers;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Services
{
    public class PublicTournamentService : IPublicTournamentService
    {
        private readonly DartDbContext _context;

        public PublicTournamentService(DartDbContext context)
        {
            _context = context;
        }

        public async Task<PublicTournamentDto?> GetTournamentByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var normalizedCode = code.Trim().ToUpperInvariant();

            var tournament = await _context.Tournaments
                .Include(t => t.Config)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == normalizedCode);

            if (tournament == null) return null;

            return await BuildPublicTournamentDtoAsync(tournament);
        }

        public async Task<PublicTournamentDto?> GetTournamentByIdAsync(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Config)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null) return null;

            return await BuildPublicTournamentDtoAsync(tournament);
        }

        private async Task<PublicTournamentDto> BuildPublicTournamentDtoAsync(Tournament tournament)
        {
            var groups = await _context.Groups
                .Where(g => g.TournamentId == tournament.Id)
                .OrderBy(g => g.Sequence)
                .AsNoTracking()
                .ToListAsync();

            var brakets = await _context.Brakets
                .Where(b => b.TournamentId == tournament.Id)
                .OrderByDescending(b => b.Sequence)
                .AsNoTracking()
                .ToListAsync();

            return new PublicTournamentDto
            {
                Id = tournament.Id,
                Name = tournament.Name,
                Code = tournament.Code,
                Description = tournament.Description,
                Status = tournament.Status,
                StartDate = tournament.StartDate,
                Config = tournament.Config?.ToTournamentConfigDto(),
                Groups = groups.Select(g => g.ToGroupDto()).ToList(),
                Brakets = brakets.Select(b => b.ToBraketDto()).ToList()
            };
        }

        public async Task<IEnumerable<TournamentDto>> GetActivePublicTournamentsAsync()
        {
            var activeTournaments = await _context.Tournaments
                .Where(t => t.Status == TournamentStatus.InProgress || t.Status == TournamentStatus.Draft)
                .OrderByDescending(t => t.StartDate)
                .Take(50)
                .AsNoTracking()
                .ToListAsync();

            return activeTournaments.Select(t => t.ToTournamentDto());
        }
    }
}
