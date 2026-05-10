using MonitorLab.Data.EntityDTOs;
using Monitor = MonitorLab.Data.Models.Monitor;
namespace MonitorLab.Core.Contracts
{
    public interface IMonitorService
    {
        Task<MonitorCatalogDTO> GetMonitorCatalogAsync();
        Task<MonitorDetailsDTO> GetMonitorDetailsAsync(Guid id);
    }
}
