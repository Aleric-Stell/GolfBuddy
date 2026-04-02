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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Courses
            builder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Sunset Hills", Location = "Irmo, SC" },
                new Course { Id = 2, Name = "Lakeview Golf Club", Location = "Columbia, SC" }
            );

            // Seed Holes
            builder.Entity<Hole>().HasData(
                new Hole { Id = 1, HoleNumber = 1, Par = 4, Yardage = 360, CourseId = 1 },
                new Hole { Id = 2, HoleNumber = 2, Par = 3, Yardage = 180, CourseId = 1 },
                new Hole { Id = 3, HoleNumber = 1, Par = 5, Yardage = 520, CourseId = 2 },
                new Hole { Id = 4, HoleNumber = 2, Par = 4, Yardage = 400, CourseId = 2 }
            );
        }

    }
}