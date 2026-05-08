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
            MonitorCatalogDTO dto = await monitorService.GetMonitorCatalogAsync();

            MonitorCatalogViewModel model = mapper.Map<MonitorCatalogViewModel>(dto);
            return View(model);
        }
    }
}
