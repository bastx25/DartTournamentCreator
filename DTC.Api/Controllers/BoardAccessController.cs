using DTC.Api.Data;
using DTC.Api.Dtos.Board;
using DTC.Api.Enums;
using DTC.Api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DTC.Api.Controllers
{
    // Public endpoints used by the QR code on a dart board.
    [Route("api/board-access")]
    [ApiController]
    public class BoardAccessController : ControllerBase
    {
        private readonly DartDbContext _context;

        public BoardAccessController(DartDbContext context)
        {
            _context = context;
        }

        [HttpGet("boards/{boardId:int}")]
        public async Task<ActionResult<BoardDto>> GetBoard([FromRoute] int boardId)
        {
            var board = await _context.Boards
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == boardId && b.IsActive);

            if (board == null)
            {
                return NotFound();
            }

            return Ok(board.ToBoardDto());
        }

        [HttpGet("boards/{boardId:int}/matches")]
        public async Task<ActionResult<IEnumerable<BoardMatchDto>>> GetBoardMatches(
            [FromRoute] int boardId)
        {
            var board = await _context.Boards
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == boardId && b.IsActive);

            if (board == null)
            {
                return NotFound();
            }


            var matches = await _context.Matches
                .AsNoTracking()
                .Include(m => m.Participants)
                    .ThenInclude(p => p.TournamentPlayer)
                        .ThenInclude(tp => tp.Player)
                .Where(m =>
                    m.BoardId == boardId)
                .OrderBy(m => m.PlannedStart)
                .ThenBy(m => m.Id)
                .ToListAsync();

            var result = matches.Select(m => new BoardMatchDto
            {
                MatchId = m.Id,
                PlannedStart = m.PlannedStart,
                ActualStart = m.ActualStart,
                Status = m.Status,
                Participants = m.Participants
                    .OrderBy(p => p.Id)
                    .Select(p => p.ToMatchParticipantDto())
                    .ToList()
            });

            return Ok(result);
        }

        [HttpPost("boards/{boardId:int}/matches/{matchId:int}/start")]
        public async Task<ActionResult<BoardMatchDto>> StartMatch(
            [FromRoute] int boardId,
            [FromRoute] int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Braket)
                 .ThenInclude(r => r.Tournament)
                .Include(m => m.Participants)
                    .ThenInclude(p => p.TournamentPlayer)
                        .ThenInclude(tp => tp.Player)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.BoardId == boardId);

            if (match == null)
            {
                return NotFound();
            }

            if (match.Status == MatchStatus.Completed)
            {
                return Conflict("Das Spiel wurde bereits abgeschlossen.");
            }

            if (match.Status == MatchStatus.Cancelled)
            {
                return Conflict("Das Spiel wurde abgebrochen.");
            }

            if (match.Status == MatchStatus.InProgress)
            {
                return Ok(match.ToBoardMatchDto());
            }

            match.Status = MatchStatus.InProgress;
            match.ActualStart = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(match.ToBoardMatchDto());

        }

        [HttpPost("boards/{boardId:int}/matches/{matchId:int}/finish")]
        public async Task<ActionResult<BoardMatchDto>> FinishMatch(
            [FromRoute] int boardId,
            [FromRoute] int matchId,
            [FromBody] FinishBoardMatchDto dto)
        {
            var match = await _context.Matches
                .Include(m => m.Braket)
                .ThenInclude(r => r.Tournament)
                .Include(m => m.Participants)
                    .ThenInclude(p => p.TournamentPlayer)
                        .ThenInclude(tp => tp.Player)
                .FirstOrDefaultAsync(m => m.Id == matchId && m.BoardId == boardId);

            if (match == null)
            {
                return NotFound();
            }

            if (match.Status == MatchStatus.Completed)
            {
                return Conflict("Das Spiel wurde bereits abgeschlossen.");
            }

            if (match.Status == MatchStatus.Cancelled)
            {
                return Conflict("Das Spiel wurde abgebrochen.");
            }

            if (match.Participants.Count != 2)
            {
                return Conflict("Ein Spiel muss genau zwei Spieler haben.");
            }

            var submitted = dto.Participants
                .GroupBy(p => p.ParticipantId)
                .ToDictionary(g => g.Key, g => g.Last().Score);

            if (submitted.Count != match.Participants.Count ||
                match.Participants.Any(p => !submitted.ContainsKey(p.Id)))
            {
                return BadRequest("Für beide Spieler muss ein Score übermittelt werden.");
            }

            var first = match.Participants.ElementAt(0);
            var second = match.Participants.ElementAt(1);
            var firstScore = submitted[first.Id];
            var secondScore = submitted[second.Id];

            if (firstScore == secondScore)
            {
                return BadRequest("Ein Unentschieden ist hier nicht zulässig. Bitte einen eindeutigen Gewinner erfassen.");
            }

            first.Score = firstScore;
            second.Score = secondScore;
            first.IsWinner = firstScore > secondScore;
            second.IsWinner = secondScore > firstScore;

            match.Status = MatchStatus.Completed;
            match.ActualEnd = DateTimeOffset.UtcNow;
            if (match.ActualStart == null)
            {
                match.ActualStart = match.ActualEnd;
            }

            await _context.SaveChangesAsync();

            return Ok(match.ToBoardMatchDto());
        }

        
    }
}
