using Microsoft.EntityFrameworkCore;
using MonitorLab.Data;
using MonitorLab.Data.Seed;

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
    }
}
