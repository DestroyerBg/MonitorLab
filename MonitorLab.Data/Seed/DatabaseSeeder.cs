using MonitorLab.Data.Enums;
using MonitorLab.Data.Models;
using MonitorLab.Data.Seed.DTOS;
using Monitor = MonitorLab.Data.Models.Monitor;

namespace MonitorLab.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            IList<PortSeedDTO> portsDto = await JsonSeederLoader.LoadPortsAsync("ports.json");

            List<Port> ports = portsDto.Select(p => new Port
            {
                Id = p.Id,
                Name = p.Name,
                Version = p.Version
            })
                .ToList();

            await context.Ports.AddRangeAsync(ports);

            List<MonitorSeedDTO> monitorsDto = await JsonSeederLoader.LoadMonitorsAsync("monitors.json");

            List<Monitor> monitors = monitorsDto.Select(m => new Monitor
            {
                Id = m.Id,
                Brand = m.Brand,
                Model = m.Model,
                Resolution = Enum.Parse<ResolutionType>(m.Resolution),
                PanelType = Enum.Parse<PanelType>(m.PanelType),
                ScreenSizeInches = m.ScreenSizeInches,
                RefreshRateHz = m.RefreshRateHz,
                ResponseTimeMs = m.ResponseTimeMs,
                BrightnessNits = m.BrightnessNits,
                ContrastRatio = m.ContrastRatio,
                Description = m.Description,
                ImageUrl = m.ImageUrl,
                ReleaseYear = m.ReleaseYear
            }).ToList();

            await context.Monitors.AddRangeAsync(monitors);
            await context.SaveChangesAsync();

            List<MonitorPortSeedDTO> monitorPortsDto = await JsonSeederLoader.LoadMonitorPortsAsync("monitorPorts.json");

            List<MonitorPort> monitorPorts = monitorPortsDto.Select(mp => new MonitorPort
            {
                MonitorId = mp.MonitorId,
                PortId = mp.PortId,
                Count = mp.Count
            }).ToList();

            await context.MonitorPorts.AddRangeAsync(monitorPorts);
            await context.SaveChangesAsync();

        }
    }
}
