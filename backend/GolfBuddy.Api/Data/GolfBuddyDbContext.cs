using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GolfBuddy.Api.Models;

namespace GolfBuddy.Api.Data
{
    public class GolfBuddyDbContext : IdentityDbContext<ApplicationUser>
    {
        public GolfBuddyDbContext(DbContextOptions<GolfBuddyDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Hole> Holes { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Shot> Shots { get; set; }

    }
}