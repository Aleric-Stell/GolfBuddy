using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GolfBuddy.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public double Handicap { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Round> Rounds { get; set; } = new List<Round>();
    }
}
