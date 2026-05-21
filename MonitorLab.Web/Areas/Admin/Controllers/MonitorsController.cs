using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Models.MonitorViewModels;

namespace MonitorLab.Web.Areas.Admin.Controllers
{
    public class MonitorsController(
        IMapper mapper, 
        IMonitorService monitorService) : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            MonitorCatalogDTO? dto = await monitorService.GetMonitorCatalogAsync();

            IEnumerable<MonitorCardViewModel> model =
                mapper.Map<IEnumerable<MonitorCardViewModel>>(dto!.Monitors);

            return View(model);
        }
    }
}
