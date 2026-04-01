using GolfBuddy.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolfBuddy.Api.Data
{
    public class GolfBuddyDbContext : IdentityDbContext<ApplicationUser>
    {
        public GolfBuddyDbContext(DbContextOptions<GolfBuddyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Round> Rounds { get; set; }
    }
}