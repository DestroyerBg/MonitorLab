using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitorLab.Data.Models;

namespace MonitorLab.Data.Configurations
{
    public class MonitorPortEntityTypeConfiguration : IEntityTypeConfiguration<MonitorPort>
    {
        public void Configure(EntityTypeBuilder<MonitorPort> builder)
        {
            builder
                .HasKey(mp => new { mp.MonitorId, mp.PortId });

            builder.HasOne(b => b.Monitor)
                .WithMany(b => b.MonitorPorts)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
