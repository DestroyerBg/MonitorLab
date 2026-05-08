using Microsoft.EntityFrameworkCore;
using MonitorLab.Data;
using MonitorLab.Data.Seed;
using MonitorLab.Web.MapperProfiles;

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
    }
}
