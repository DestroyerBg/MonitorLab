using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonitorLab.Core.Contracts;
using MonitorLab.Data;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Data.Enums;
using MonitorLab.Data.Models;
using Monitor = MonitorLab.Data.Models.Monitor;
namespace MonitorLab.Core.Services
{
    public class MonitorService(
        IMapper mapper,
        ApplicationDbContext dbContext) : IMonitorService
    {
        public async Task<MonitorCatalogDTO?> GetMonitorCatalogAsync()
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

        public async Task<CompareDTO> GetMonitorComparisonAsync(IList<Guid> ids)
        {
            CompareDTO dto = new CompareDTO();

            foreach (var id in ids)
            {
                Monitor? monitor = await GetMonitorByIdWithPortsAsync(id);
                if (monitor == null)
                {
                   continue;
                }

                MonitorComparisonCardDTO monitorDto = mapper.Map<MonitorComparisonCardDTO>(monitor);

                monitorDto.Ports = MapMonitorPorts(monitor.MonitorPorts);

                dto.Monitors.Add(monitorDto);

            }
            return dto;
        }

        public async Task<MonitorDetailsDTO?> GetMonitorDetailsAsync(Guid id)
        {
            Monitor? monitor = await GetMonitorByIdWithPortsAsync(id);

            if (monitor == null)
            {
                return null;
            }

            MonitorDetailsDTO details = mapper.Map<MonitorDetailsDTO>(monitor);

            details.Ports = MapMonitorPorts(monitor.MonitorPorts);

            return details;

        }

        public async Task<IEnumerable<SelectListItem>> GetDistinctResolutions()
        {
            IEnumerable<SelectListItem> resolutions = await dbContext.Monitors
                .Select(m => m.Resolution)
                .Distinct()
                .Select(r => new SelectListItem
                {
                    Text = r.ToString(),
                    Value = r.ToString()
                })
                .ToListAsync();

            return resolutions;
        }
        public async Task<IEnumerable<SelectListItem>> GetDistinctPanelTypes()
        {
            IEnumerable<SelectListItem> panelTypes = await dbContext.Monitors
                .Select(m => m.PanelType)
                .Distinct()
                .Select(r => new SelectListItem
                {
                    Text = r.ToString(),
                    Value = r.ToString()
                })
                .ToListAsync();

            return panelTypes;
        }

        public async Task<Guid> CreateMonitorAsync(MonitorCreateDTO monitorCreateDTO)
        {
            Monitor monitor = mapper.Map<Monitor>(monitorCreateDTO);

            monitor.Id = Guid.NewGuid();

            monitor.MonitorPorts = monitorCreateDTO.Ports.Select(p => new MonitorPort
            {
                PortId = p.PortId,
                MonitorId = monitor.Id,
                Count = p.Count
            }).ToList();

            await dbContext.Monitors.AddAsync(monitor);
            await dbContext.SaveChangesAsync();

            return monitor.Id;
        }

        public async Task<IList<MonitorPortCreateDTO>> GetPortsForCreateAsync()
        {
            return await dbContext.Ports
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Version)
                .Select(p => new MonitorPortCreateDTO
                {
                    PortId = p.Id,
                    Name = p.Name,
                    Version = p.Version,
                    Count = 1
                })
                .ToListAsync();
        }

        public async Task UpdateMonitorImageAsync(Guid monitorId, string imageUrl)
        {
            Monitor? monitor = await dbContext.Monitors.FindAsync(monitorId);

            if (monitor == null)
            {
                return;
            }

            monitor.ImageUrl = imageUrl;

            await dbContext.SaveChangesAsync();
        }

        public async Task<string?> DeleteMonitorAsync(Guid id)
        {
            Monitor? monitor = await dbContext.Monitors
                .Include(m => m.MonitorPorts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monitor == null)
            {
                return null;
            }

            string? imageUrl = monitor.ImageUrl;

            dbContext.MonitorPorts.RemoveRange(monitor.MonitorPorts);
            dbContext.Monitors.Remove(monitor);

            await dbContext.SaveChangesAsync();

            return imageUrl;
        }

        public async Task<MonitorEditDTO?> GetMonitorForEditAsync(Guid id)
        {
            Monitor? monitor = await dbContext.Monitors
                                .Include(m => m.MonitorPorts)
                                .ThenInclude(m => m.Port)
                                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitor == null)
            {
                return null;
            }

            MonitorEditDTO dto = mapper.Map<MonitorEditDTO>(monitor);
            return dto;
        }

        public async Task<IList<MonitorPortCreateDTO>> GetPortsForEditAsync(Guid monitorId)
        {
            List<Guid> selectedPortIds = await dbContext.MonitorPorts
                                        .Where(mp => mp.MonitorId == monitorId)
                                        .Select(mp => mp.PortId)
                                        .ToListAsync();

            Dictionary<Guid, int> selectedPortCounts = await dbContext.MonitorPorts
                .Where(mp => mp.MonitorId == monitorId)
                .ToDictionaryAsync(mp => mp.PortId, mp => mp.Count);

            return await dbContext.Ports
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Version)
                .Select(p => new MonitorPortCreateDTO
                {
                    PortId = p.Id,
                    Name = p.Name,
                    Version = p.Version,
                    IsSelected = selectedPortIds.Contains(p.Id),
                    Count = selectedPortCounts.ContainsKey(p.Id)
                        ? selectedPortCounts[p.Id]
                        : 1
                })
                .ToListAsync();
        }

        public async Task<bool> EditMonitorAsync(MonitorEditDTO dto)
        {
            Monitor? monitor = await dbContext.Monitors
                .Include(m => m.MonitorPorts)
                .FirstOrDefaultAsync(m => m.Id == dto.Id);

            if (monitor == null)
            {
                return false;
            }

            monitor.Brand = dto.Brand;
            monitor.Model = dto.Model;
            monitor.Resolution = Enum.Parse<ResolutionType>(dto.Resolution);
            monitor.PanelType = Enum.Parse<PanelType>(dto.PanelType);
            monitor.ScreenSizeInches = dto.ScreenSizeInches;
            monitor.RefreshRateHz = dto.RefreshRateHz;
            monitor.ResponseTimeMs = dto.ResponseTimeMs;
            monitor.BrightnessNits = dto.BrightnessNits;
            monitor.ContrastRatio = dto.ContrastRatio;
            monitor.Description = dto.Description;
            monitor.ReleaseYear = dto.ReleaseYear;

            dbContext.MonitorPorts.RemoveRange(monitor.MonitorPorts);

            monitor.MonitorPorts = dto.Ports
                .Where(p => p.IsSelected)
                .Select(p => new MonitorPort
                {
                    MonitorId = monitor.Id,
                    PortId = p.PortId,
                    Count = p.Count
                })
                .ToList();

            await dbContext.SaveChangesAsync();

            return true;
        }

        private async Task<Monitor?> GetMonitorByIdWithPortsAsync(Guid id)
        {
            return await dbContext.Monitors
                .Include(m => m.MonitorPorts)
                .ThenInclude(mp => mp.Port)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        private IEnumerable<MonitorPortDetailsDTO> MapMonitorPorts(IEnumerable<MonitorPort> monitorPorts)
        {
            return monitorPorts.Select(mp =>
            {
                MonitorPortDetailsDTO portDto = mapper.Map<MonitorPortDetailsDTO>(mp.Port);
                portDto.Count = mp.Count;

                return portDto;
            })
              .ToList();
        }

    }
}
