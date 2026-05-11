using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core;
using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Models.MonitorViewModels;
namespace MonitorLab.Web.Controllers
{
    public class MonitorsController(
        IMapper mapper,
        IMonitorService monitorService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            MonitorCatalogDTO? dto = await monitorService.GetMonitorCatalogAsync();

            MonitorCatalogViewModel model = mapper.Map<MonitorCatalogViewModel>(dto);
            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            MonitorDetailsDTO? dto = await monitorService.GetMonitorDetailsAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            MonitorDetailsViewModel model = mapper.Map<MonitorDetailsViewModel>(dto);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(
         string? searchTerm,
         string? brand,
         string? resolution,
         string? panelType,
         int? minRefreshRate)
        {
            IEnumerable<MonitorCardDto> dtos = await monitorService.GetMonitorCatalogAsync(searchTerm, brand, resolution, panelType, minRefreshRate);

            IEnumerable<MonitorCardViewModel> model = mapper.Map<IEnumerable<MonitorCardViewModel>>(dtos);

            return PartialView("_MonitorCardsPartial", model);
        }
    }
}
