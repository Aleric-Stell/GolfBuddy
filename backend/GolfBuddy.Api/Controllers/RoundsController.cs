using GolfBuddy.Api.Contracts.Rounds;
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
    public class RoundsController : ControllerBase
    {
        private readonly GolfBuddyDbContext _context;

        public RoundsController(GolfBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoundResponse>>> GetRounds()
        {
            var userId = User.GetUserId();
            var query = _context.Rounds
                .AsNoTracking()
                .Include(r => r.Course)
                    .ThenInclude(c => c!.Holes)
                .Include(r => r.Shots)
                .AsQueryable();

            if (!User.IsAdmin())
            {
                query = query.Where(r => r.UserId == userId);
            }

            var rounds = await query
                .OrderByDescending(r => r.DatePlayed)
                .ToListAsync();

            return Ok(rounds.Select(MapRound));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoundDetailResponse>> GetRound(int id)
        {
            var round = await _context.Rounds
                .AsNoTracking()
                .Include(r => r.Course)
                    .ThenInclude(c => c!.Holes)
                .Include(r => r.Shots)
                    .ThenInclude(s => s.Hole)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (round == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            return Ok(MapRoundDetail(round));
        }

        [HttpGet("{id}/scorecard")]
        public async Task<ActionResult<RoundScorecardResponse>> GetRoundScorecard(int id)
        {
            var round = await _context.Rounds
                .AsNoTracking()
                .Include(r => r.Course)
                    .ThenInclude(c => c!.Holes)
                .Include(r => r.Shots)
                    .ThenInclude(s => s.Hole)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (round == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            return Ok(MapScorecard(round));
        }

        [HttpPost]
        public async Task<ActionResult<RoundResponse>> CreateRound(CreateRoundRequest request)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var course = await _context.Courses
                .AsNoTracking()
                .Include(c => c.Holes)
                .FirstOrDefaultAsync(c => c.Id == request.CourseId);

            if (course == null)
            {
                return BadRequest("Invalid CourseId");
            }

            if (course.Holes.Count == 0)
            {
                return BadRequest("Cannot create a round for a course without holes.");
            }

            var round = new Round
            {
                DatePlayed = request.DatePlayed,
                UserId = userId,
                CourseId = request.CourseId,
                User = null!,
                Course = null!
            };

            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();

            var createdRound = await _context.Rounds
                .AsNoTracking()
                .Include(r => r.Course)
                    .ThenInclude(c => c!.Holes)
                .Include(r => r.Shots)
                .FirstAsync(r => r.Id == round.Id);

            return CreatedAtAction(nameof(GetRound), new { id = round.Id }, MapRound(createdRound));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRound(int id, UpdateRoundRequest request)
        {
            var round = await _context.Rounds.FindAsync(id);
            if (round == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            round.DatePlayed = request.DatePlayed;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRound(int id)
        {
            var round = await _context.Rounds.FindAsync(id);
            if (round == null)
            {
                return NotFound();
            }

            if (!CanAccessRound(round))
            {
                return Forbid();
            }

            _context.Rounds.Remove(round);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CanAccessRound(Round round)
        {
            return User.IsAdmin() || round.UserId == User.GetUserId();
        }

        private static RoundResponse MapRound(Round round)
        {
            var totalPar = round.Course?.Holes.Sum(h => h.Par) ?? 0;
            var totalStrokes = round.Shots.Count;

            return new RoundResponse
            {
                Id = round.Id,
                DatePlayed = round.DatePlayed,
                UserId = round.UserId,
                CourseId = round.CourseId ?? 0,
                CourseName = round.Course?.Name,
                HoleCount = round.Course?.Holes.Count ?? 0,
                ShotCount = totalStrokes,
                TotalStrokes = totalStrokes,
                TotalPar = totalPar,
                ScoreToPar = totalStrokes - totalPar
            };
        }

        private static RoundDetailResponse MapRoundDetail(Round round)
        {
            var holes = round.Course?.Holes
                .OrderBy(h => h.HoleNumber)
                .ToList() ?? new List<Hole>();

            return new RoundDetailResponse
            {
                Id = round.Id,
                DatePlayed = round.DatePlayed,
                UserId = round.UserId,
                CourseId = round.CourseId ?? 0,
                CourseName = round.Course?.Name,
                ShotCount = round.Shots.Count,
                HoleCount = holes.Count,
                TotalStrokes = round.Shots.Count,
                TotalPar = holes.Sum(h => h.Par),
                ScoreToPar = round.Shots.Count - holes.Sum(h => h.Par),
                Holes = holes
                    .Select(h => new RoundHoleSummaryResponse
                    {
                        HoleId = h.Id,
                        HoleNumber = h.HoleNumber,
                        Par = h.Par,
                        Yardage = h.Yardage,
                        Strokes = round.Shots.Count(s => s.HoleId == h.Id),
                        ScoreToPar = round.Shots.Count(s => s.HoleId == h.Id) - h.Par
                    })
                    .ToList(),
                Shots = round.Shots
                    .OrderBy(s => s.Hole.HoleNumber)
                    .ThenBy(s => s.ShotNumber)
                    .Select(MapShot)
                    .ToList()
            };
        }

        private static RoundScorecardResponse MapScorecard(Round round)
        {
            var holes = round.Course?.Holes
                .OrderBy(h => h.HoleNumber)
                .Select(h => new RoundHoleSummaryResponse
                {
                    HoleId = h.Id,
                    HoleNumber = h.HoleNumber,
                    Par = h.Par,
                    Yardage = h.Yardage,
                    Strokes = round.Shots.Count(s => s.HoleId == h.Id),
                    ScoreToPar = round.Shots.Count(s => s.HoleId == h.Id) - h.Par
                })
                .ToList() ?? new List<RoundHoleSummaryResponse>();

            var totalPar = holes.Sum(h => h.Par);
            var totalStrokes = holes.Sum(h => h.Strokes);

            return new RoundScorecardResponse
            {
                RoundId = round.Id,
                DatePlayed = round.DatePlayed,
                CourseName = round.Course?.Name ?? "Unknown Course",
                TotalPar = totalPar,
                TotalStrokes = totalStrokes,
                ScoreToPar = totalStrokes - totalPar,
                Holes = holes
            };
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
