using GolfBuddy.Api.Data;
using GolfBuddy.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // GET: api/rounds
        [HttpGet]
        public IActionResult GetRounds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rounds = _context.Rounds.Where(r => r.UserId == userId).ToList();
            return Ok(rounds);
        }

        //POST: api/rounds
        [HttpPost]
        public IActionResult AddRound([FromBody] Round round)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            round.UserId = userId ?? throw new InvalidOperationException("UserId missing");

            _context.Rounds.Add(round);
            _context.SaveChanges();

            return Ok(round);
        }
    }
}