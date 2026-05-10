using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MonitorLab.Core.Contracts;
using MonitorLab.Data;
using MonitorLab.Data.EntityDTOs;
using Monitor = MonitorLab.Data.Models.Monitor;
namespace MonitorLab.Core.Services
{
    public class MonitorService(
        IMapper mapper,
        ApplicationDbContext dbContext) : IMonitorService
    {
        public async Task<MonitorCatalogDTO> GetMonitorCatalogAsync()
        {
            IList<MonitorCardDto> monitors = await dbContext.Monitors
                .Select(m => mapper.Map<MonitorCardDto>(m)).ToListAsync();

            MonitorCatalogDTO catalog = new MonitorCatalogDTO();

            catalog.Monitors = monitors;

            return catalog;
        }

        public async Task<MonitorDetailsDTO> GetMonitorDetailsAsync(Guid id)
        {
            if (!await CheckId(id))
            {
                return null;
            }

            Monitor? monitor = await dbContext.Monitors
                .Include(m => m.MonitorPorts)
                .ThenInclude(mp => mp.Port)
                .FirstOrDefaultAsync(m => m.Id == id);

            MonitorDetailsDTO details = mapper.Map<MonitorDetailsDTO>(monitor);

            foreach (var monitorPort in monitor.MonitorPorts)
            {
                MonitorPortDetailsDTO portDetails = mapper.Map<MonitorPortDetailsDTO>(monitorPort.Port);
                portDetails.Count = monitorPort.Count;
                details.Ports = details.Ports.Append(portDetails);
            } 

            return details;
        }

        private async Task<bool> CheckId(Guid id)
        {
            return await dbContext.Monitors.AnyAsync(m => m.Id == id);
        }
    }
}
