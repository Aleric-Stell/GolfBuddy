using Microsoft.AspNetCore.Mvs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Api.Data;
using Microsoft.Api.Models;

namespace GolfBuddy.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolesController : ControllerBase
    {
        private readonly GolfBuddyDbContext _context;

        public HolesController(GolfBuddyDbContext context)
        {
            _context = context;
        }

        // GET: api/Holes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hole>>> GetHoles()
        {
            return await _context.Holes.ToListAsync();
        }

        // GET: api/Holes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hole>> GetHole(int id)
        {
            var hole = await _context.Holes.FindAsync(id);

            if (hole == null)
                return NotFound();

            return hole;
        }

        // GET: api/Holes/course/1
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Hole>>> GetHolesByCourse(int courseId)
        {
            var holes = await _context.Holes
                .Where(h => h.CourseId == courseId)
                .OrderBy(h => h.HoleNumber)
                .ToListAsync();

            return holes;
        }

        // POST: api/Holes
        [HttpPost]
        public async Task<ActionResult<Hole>> CreateHole(Hole hole)
        {

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == hole.CourseId);
            if (!courseExists)
                return BadRequest("Invalid CourseId");

            _context.Holes.Add(hole);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHole), new { id = hole.Id }, hole);
        }

        // PUT: api/Holes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHole(int id, Hole hole)
        {
            if (id != hole.Id)
                return BadRequest();

            var existingHole = await _context.Holes.FindAsync(id);

            if (existingHole == null)
                return NotFound();

            existingHole.HoleNumber = hole.HoleNumber;
            existingHole.Par = hole.Par;
            existingHole.Yardage = hole.Yardage;
            existingHole.CourseId = hole.CourseId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Holes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHole(int id)
        {
            var hole = await _context.Holes.FindAsync(id);

            if (hole == null)
                return NotFound();

            _context.Holes.Remove(hole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}