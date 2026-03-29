using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortEntity = MonitorLab.Data.Models.Port;
using MonitorEntity = MonitorLab.Data.Models.Monitor;
using MonitorPortEntity = MonitorLab.Data.Models.MonitorPort;

namespace MonitorLab.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MonitorEntity> Monitors { get; set; } = null!;
        public DbSet<PortEntity> Ports { get; set; } = null!;

        public DbSet<MonitorPortEntity> MonitorPorts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}