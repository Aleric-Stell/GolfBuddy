using GolfBuddy.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace GolfBuddy.Api.Infrastructure
{
    public static class SeedData
    {
        private const string AdminEmail = "admin@golfbuddy.local";
        private const string AdminPassword = "GolfBuddy123!";
        private const string PlayerEmail = "player@golfbuddy.local";
        private const string PlayerPassword = "GolfBuddy123!";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await EnsureRoleAsync(roleManager, AuthConstants.AdminRole);
            await EnsureRoleAsync(roleManager, AuthConstants.PlayerRole);

            await EnsureUserAsync(userManager, AdminEmail, "admin", AdminPassword, AuthConstants.AdminRole, 8.4);
            await EnsureUserAsync(userManager, PlayerEmail, "player", PlayerPassword, AuthConstants.PlayerRole, 18.7);
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string userName,
            string password,
            string role,
            double handicap)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = userName,
                    EmailConfirmed = true,
                    Handicap = handicap
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed user {email}: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
