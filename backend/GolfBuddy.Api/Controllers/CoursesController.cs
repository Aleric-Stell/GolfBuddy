using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GolfBuddy.Api.Data;
using GolfBuddy.Api.Models;

namespace GolfBuddy.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly GolfBuddyDbContext _context;

        public CoursesController(GolfBuddyDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            return await _context.Courses.ToListAsync();
        }

        // GET: api/Courses/5
        [HttpGet]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound();
            
            return course;
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, Course course)
        {
            if (id != course.Id)
                return BadRequest();

            var existingCourse = await _context.Courses.FindAsync(id);

            if (existingCourse == null)
                return NotFound();

            // Update fields
            existingCourse.Name = course.Name;
            existingCourse.Location = course.Location;

            await _context.SaveChangesAsync();

            return NoContent(); // standard for PUT
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }        
    }
}