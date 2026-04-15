using GolfBuddy.Api.Contracts.Shots;
using GolfBuddy.Api.Data;
using GolfBuddy.Api.Infrastructure;
using GolfBuddy.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GolfBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShotsController : ControllerBase
    {
        private readonly GolfBuddyDbContext _context;

        public ShotsController(GolfBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShotResponse>>> GetShots()
        {
            var userId = User.GetUserId();
            var query = _context.Shots
                .AsNoTracking()
                .Include(s => s.Hole)
                .Include(s => s.Round)
                .AsQueryable();

            if (!User.IsAdmin())
            {
                query = query.Where(s => s.Round.UserId == userId);
            }

            var shots = await query
                .OrderBy(s => s.RoundId)
                .ThenBy(s => s.Hole.HoleNumber)
                .ThenBy(s => s.ShotNumber)
                .ToListAsync();

            return Ok(shots.Select(MapShot));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShotResponse>> GetShot(int id)
        {
            var shot = await _context.Shots
                .AsNoTracking()
                .Include(s => s.Hole)
                .Include(s => s.Round)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shot == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(shot.Round))
            {
                return Forbid();
            }

            return Ok(MapShot(shot));
        }

        [HttpGet("round/{roundId}")]
        public async Task<ActionResult<IEnumerable<ShotResponse>>> GetShotsByRound(int roundId)
        {
            var round = await _context.Rounds
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roundId);

            if (round == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            var shots = await _context.Shots
                .AsNoTracking()
                .Include(s => s.Hole)
                .Include(s => s.Round)
                .Where(s => s.RoundId == roundId)
                .OrderBy(s => s.Hole.HoleNumber)
                .ThenBy(s => s.ShotNumber)
                .ToListAsync();

            return Ok(shots.Select(MapShot));
        }

        [HttpGet("round/{roundId}/hole/{holeId}")]
        public async Task<ActionResult<IEnumerable<ShotResponse>>> GetShotsByRoundAndHole(int roundId, int holeId)
        {
            var round = await _context.Rounds
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roundId);

            if (round == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            var shots = await _context.Shots
                .AsNoTracking()
                .Include(s => s.Hole)
                .Include(s => s.Round)
                .Where(s => s.RoundId == roundId && s.HoleId == holeId)
                .OrderBy(s => s.ShotNumber)
                .ToListAsync();

            return Ok(shots.Select(MapShot));
        }

        [HttpPost]
        public async Task<ActionResult<ShotResponse>> CreateShot(CreateShotRequest request)
        {
            var round = await _context.Rounds
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.RoundId);
            if (round == null)
            {
                return BadRequest("Invalid RoundId");
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            var hole = await _context.Holes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == request.HoleId);
            if (hole == null)
            {
                return BadRequest("Invalid HoleId");
            }

            if (round.CourseId.HasValue && hole.CourseId != round.CourseId.Value)
            {
                return BadRequest("Hole does not belong to the round's course");
            }

            var shot = new Shot
            {
                HoleId = request.HoleId,
                RoundId = request.RoundId,
                ShotNumber = request.ShotNumber,
                DistanceYards = request.DistanceYards,
                Club = request.Club.Trim(),
                Result = request.Result,
                Hole = null!,
                Round = null!
            };

            _context.Shots.Add(shot);
            await _context.SaveChangesAsync();

            var createdShot = await _context.Shots
                .AsNoTracking()
                .Include(s => s.Hole)
                .FirstAsync(s => s.Id == shot.Id);

            return CreatedAtAction(nameof(GetShot), new { id = shot.Id }, MapShot(createdShot));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShot(int id, UpdateShotRequest request)
        {
            var shot = await _context.Shots.FindAsync(id);
            if (shot == null)
            {
                return NotFound();
            }

            var round = await _context.Rounds
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == shot.RoundId);

            if (round == null)
            {
                return BadRequest("Invalid RoundId");
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            var hole = await _context.Holes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == request.HoleId);
            if (hole == null)
            {
                return BadRequest("Invalid HoleId");
            }

            if (round?.CourseId.HasValue == true && hole.CourseId != round.CourseId.Value)
            {
                return BadRequest("Hole does not belong to the round's course");
            }

            shot.HoleId = request.HoleId;
            shot.ShotNumber = request.ShotNumber;
            shot.DistanceYards = request.DistanceYards;
            shot.Club = request.Club.Trim();
            shot.Result = request.Result;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShot(int id)
        {
            var shot = await _context.Shots.FindAsync(id);
            if (shot == null)
            {
                return NotFound();
            }

            var round = await _context.Rounds
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == shot.RoundId);

            if (round == null)
            {
                return BadRequest("Invalid RoundId");
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            _context.Shots.Remove(shot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CanAccessRound(Round round)
        {
            return User.IsAdmin() || round.UserId == User.GetUserId();
        }

        private static ShotResponse MapShot(Shot shot)
        {
            return new ShotResponse
            {
                Id = shot.Id,
                RoundId = shot.RoundId,
                HoleId = shot.HoleId,
                HoleNumber = shot.Hole?.HoleNumber ?? 0,
                ShotNumber = shot.ShotNumber,
                DistanceYards = shot.DistanceYards,
                Club = shot.Club,
                Result = shot.Result
            };
        }
    }
}
