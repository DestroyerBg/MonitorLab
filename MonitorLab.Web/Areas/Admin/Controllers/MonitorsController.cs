using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Models.MonitorViewModels;

namespace MonitorLab.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MonitorsController(
        IMapper mapper, 
        IMonitorService monitorService) : Controller
    {
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            MonitorCatalogDTO? dto = await monitorService.GetMonitorCatalogAsync();

            IEnumerable<MonitorCardViewModel> model =
                mapper.Map<IEnumerable<MonitorCardViewModel>>(dto!.Monitors);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            MonitorCreateViewModel model = new MonitorCreateViewModel();

            model.Resolutions = await monitorService.GetDistinctResolutions();
            model.PanelTypes = await monitorService.GetDistinctPanelTypes();
            return View(model);
        }
    }
}
