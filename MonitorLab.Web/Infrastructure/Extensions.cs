using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonitorLab.Data;
using MonitorLab.Data.Seed;
using MonitorLab.Web.MapperProfiles;
using System.Text.Json;
namespace MonitorLab.Web.Infrastructure
{
    public static class Extensions
    {
        public static async void CreateDatabase(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            if (!await dbContext.Monitors.AnyAsync())
            {
                await DatabaseSeeder.SeedAsync(dbContext);
            }
        }

        public static IServiceCollection RegisterAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MonitorProfiles).Assembly));
            return services;
        }

        public static void SetObject<T>(
         this ISession session,
         string key,
         T value)
        {
            string json = JsonSerializer.Serialize(value);

            session.SetString(key, json);
        }

        public static T? GetObject<T>(
            this ISession session,
            string key)
        {
            string? json = session.GetString(key);

            if (json == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        public static async Task<WebApplication> SeedAdminAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            await AdminSeeder.SeedAsync(userManager, roleManager, configuration);

            return app;
        }

    }
}
