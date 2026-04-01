using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GolfBuddy.Api.Models;

namespace GolfBuddy.Api.Data
{
    public class GolfBuddyDbContext : IdentityDbContext<ApplicationUser>
    {
        public GolfBuddyDbContext(DbContextOptions<GolfBuddyDbContext> options)
            : base(options) { }

        // Example: public DbSet<Course> Courses { get; set; }
    }
}