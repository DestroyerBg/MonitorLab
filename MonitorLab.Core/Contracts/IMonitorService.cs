using Microsoft.AspNetCore.Mvc.Rendering;
using MonitorLab.Data.EntityDTOs;
using Monitor = MonitorLab.Data.Models.Monitor;
namespace MonitorLab.Core.Contracts
{
    public interface IMonitorService
    {
        Task<MonitorCatalogDTO?> GetMonitorCatalogAsync();
        Task<IEnumerable<MonitorCardDto>> GetMonitorCatalogAsync(
            string? searchTerm,
            string? brand,
            string? resolution,
            string? panelType,
            int? minRefreshRate);
        Task<MonitorDetailsDTO?> GetMonitorDetailsAsync(Guid id);
        Task<CompareDTO> GetMonitorComparisonAsync(IList<Guid> ids);
        Task<IEnumerable<SelectListItem>> GetDistinctPanelTypes();
        Task<IEnumerable<SelectListItem>> GetDistinctResolutions();
        Task<IList<MonitorPortCreateDTO>> GetPortsForCreateAsync();
        Task<Guid> CreateMonitorAsync(MonitorCreateDTO monitorCreateDTO);
        Task UpdateMonitorImageAsync(Guid monitorId, string imageUrl);
        Task<MonitorEditDTO?> GetMonitorForEditAsync(Guid id);
        Task<MonitorDeleteResultDTO> DeleteMonitorAsync(Guid id);

        Task<bool> EditMonitorAsync(MonitorEditDTO dto);
    }
}
