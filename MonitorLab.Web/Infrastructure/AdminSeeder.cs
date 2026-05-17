using Microsoft.AspNetCore.Identity;

namespace MonitorLab.Web.Infrastructure
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
        {
            const string adminRole = "Admin";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            string email = configuration["AdminUser:Email"] ?? "admin@monitorlab.local";
            string username = configuration["AdminUser:Username"] ?? "admin";
            string? password = configuration["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("Admin password is not configured.");
            }

            IdentityUser? admin = await userManager.FindByNameAsync(username);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                IdentityResult createResult = await userManager.CreateAsync(admin, password);

                if (!createResult.Succeeded)
                {
                    string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Admin user creation failed: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(admin, adminRole))
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }
        }
    }
}
