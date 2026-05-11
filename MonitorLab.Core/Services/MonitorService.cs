using AutoMapper;
using AutoMapper.QueryableExtensions;
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

            catalog.Brands = monitors.Select(b => b.Brand).Distinct().ToList();
            catalog.Resolutions = monitors.Select(r => r.Resolution).Distinct().ToList();
            catalog.PanelTypes = monitors.Select(p => p.PanelType).Distinct().ToList();

            return catalog;
        }

        public async Task<IEnumerable<MonitorCardDto>> GetMonitorCatalogAsync(string? searchTerm, string? brand, string? resolution, string? panelType, int? minRefreshRate)
        {
            IQueryable<Monitor>? query = dbContext.Monitors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m =>
                    m.Brand.Contains(searchTerm) ||
                    m.Model.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(m => m.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(resolution))
            {
                query = query.Where(m => m.Resolution.ToString() == resolution);
            }

            if (!string.IsNullOrWhiteSpace(panelType))
            {
                query = query.Where(m => m.PanelType.ToString() == panelType);
            }

            if (minRefreshRate.HasValue)
            {
                query = query.Where(m => m.RefreshRateHz >= minRefreshRate.Value);
            }

            IList<MonitorCardDto> monitors = await query
                .ProjectTo<MonitorCardDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return monitors;
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
