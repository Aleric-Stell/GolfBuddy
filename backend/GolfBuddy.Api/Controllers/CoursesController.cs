using GolfBuddy.Api.Contracts.Courses;
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
    public class CoursesController : ControllerBase
    {
        private readonly GolfBuddyDbContext _context;

        public CoursesController(GolfBuddyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseSummaryResponse>>> GetCourse()
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .Include(c => c.Holes)
                .OrderBy(c => c.Name)
                .Select(c => new CourseSummaryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    HoleCount = c.Holes.Count,
                    TotalPar = c.Holes.Sum(h => h.Par)
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDetailResponse>> GetCourse(int id)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .Include(c => c.Holes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(MapCourseDetail(course));
        }

        [HttpPost]
        [Authorize(Roles = AuthConstants.AdminRole)]
        public async Task<ActionResult<CourseDetailResponse>> CreateCourse(CreateCourseRequest request)
        {
            var validationResult = ValidateHoles(request.Holes);
            if (validationResult != null)
            {
                return BadRequest(validationResult);
            }

            var course = new Course
            {
                Name = request.Name.Trim(),
                Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
                Holes = request.Holes
                    .OrderBy(h => h.HoleNumber)
                    .Select(h => new Hole
                    {
                        HoleNumber = h.HoleNumber,
                        Par = h.Par,
                        Yardage = h.Yardage
                    })
                    .ToList()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var createdCourse = await _context.Courses
                .AsNoTracking()
                .Include(c => c.Holes)
                .FirstAsync(c => c.Id == course.Id);

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, MapCourseDetail(createdCourse));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AuthConstants.AdminRole)]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseRequest request)
        {
            var validationResult = ValidateHoles(request.Holes);
            if (validationResult != null)
            {
                return BadRequest(validationResult);
            }

            var existingCourse = await _context.Courses
                .Include(c => c.Holes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCourse == null)
            {
                return NotFound();
            }

            existingCourse.Name = request.Name.Trim();
            existingCourse.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();

            _context.Holes.RemoveRange(existingCourse.Holes);
            existingCourse.Holes = request.Holes
                .OrderBy(h => h.HoleNumber)
                .Select(h => new Hole
                {
                    CourseId = id,
                    HoleNumber = h.HoleNumber,
                    Par = h.Par,
                    Yardage = h.Yardage
                })
                .ToList();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AuthConstants.AdminRole)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string? ValidateHoles(List<CourseHoleRequest> holes)
        {
            if (holes == null || holes.Count == 0)
            {
                return "A course must include at least one hole.";
            }

            if (holes.GroupBy(h => h.HoleNumber).Any(g => g.Count() > 1))
            {
                return "Hole numbers must be unique.";
            }

            if (holes.Any(h => h.HoleNumber <= 0 || h.Par <= 0 || h.Yardage < 0))
            {
                return "Each hole must have a positive hole number and par, and a non-negative yardage.";
            }

            return null;
        }

        private static CourseDetailResponse MapCourseDetail(Course course)
        {
            return new CourseDetailResponse
            {
                Id = course.Id,
                Name = course.Name,
                Location = course.Location,
                HoleCount = course.Holes.Count,
                TotalPar = course.Holes.Sum(h => h.Par),
                Holes = course.Holes
                    .OrderBy(h => h.HoleNumber)
                    .Select(h => new CourseHoleResponse
                    {
                        Id = h.Id,
                        HoleNumber = h.HoleNumber,
                        Par = h.Par,
                        Yardage = h.Yardage
                    })
                    .ToList()
            };
        }
    }
}
