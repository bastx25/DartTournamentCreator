using DTC.Api.Data;
using DTC.Api.Enums;
using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Services
{
    public class MatchMakerService : IMatchMakerService
    {
        private DartDbContext _context;

        public MatchMakerService(DartDbContext dartDbContext)
        {
            _context = dartDbContext;
        }

        public async Task<bool> GenerateRandomGroupsAsync(int tournamentId, int groupSize, List<int>? selectedPlayerIds = null)
        {
            var tournament = await _context.Tournaments.FindAsync(tournamentId);
            if (tournament == null) return false;

            // 1. Spieler laden & per Zufall mischen (Fisher-Yates Shuffle)
            var query = _context.Players.AsQueryable();
            if (selectedPlayerIds != null && selectedPlayerIds.Any())
            {
                query = query.Where(p => selectedPlayerIds.Contains(p.Id));
            }

            var players = await query.ToListAsync();
            var random = new Random();
            var shuffledPlayers = players.OrderBy(_ => random.Next()).ToList();

            // 2. Erste Runde für die Gruppenphase anlegen (Falls noch nicht vorhanden)
            var defaultLocation = await _context.Locations.FirstOrDefaultAsync();
            if (defaultLocation == null) return false;

            var round = new Round
            {
                TournamentId = tournamentId,
                LocationId = defaultLocation.Id,
                Sequence = 1,
                Name = "Gruppenphase",
                PlannedStart = DateTimeOffset.UtcNow,
                Status = RoundStatus.Scheduled
            };
            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();

            // 3. Gruppen bilden und Matches generieren
            for (int i = 0; i < shuffledPlayers.Count; i += groupSize)
            {
                var groupPlayers = shuffledPlayers.Skip(i).Take(groupSize).ToList();

                // Jeder gegen jeden innerhalb der Gruppe (Round Robin)
                for (int p1 = 0; p1 < groupPlayers.Count; p1++)
                {
                    for (int p2 = p1 + 1; p2 < groupPlayers.Count; p2++)
                    {
                        var match = new Match
                        {
                            RoundId = round.Id,
                            Status = MatchStatus.Scheduled
                        };
                        _context.Matches.Add(match);
                        await _context.SaveChangesAsync();

                        _context.MatchParticipants.AddRange(
                            new MatchParticipant { MatchId = match.Id, PlayerId = groupPlayers[p1].Id },
                            new MatchParticipant { MatchId = match.Id, PlayerId = groupPlayers[p2].Id }
                        );
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
