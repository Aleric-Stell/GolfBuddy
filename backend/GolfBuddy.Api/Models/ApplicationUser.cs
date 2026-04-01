using Microsoft.AspNetCore.Identity;

namespace GolfBuddy.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Handicap { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}