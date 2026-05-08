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
    }
}
