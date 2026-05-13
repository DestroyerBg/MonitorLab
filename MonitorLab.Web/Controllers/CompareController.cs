using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Infrastructure;
using MonitorLab.Web.Models.CompareViewModels;
namespace MonitorLab.Web.Controllers
{
    public class CompareController(
        IMonitorService monitorService,
        IMapper mapper) : Controller
    {
        public IActionResult Add(Guid id)
        {
            IList<Guid> ids = HttpContext.Session.GetObject<List<Guid>>("CompareMonitors") ?? new();

            if (!ids.Contains(id) && ids.Count < 3)
            {
                ids.Add(id);
            }

            HttpContext.Session.SetObject("CompareMonitors", ids);

            return RedirectToAction("Index", "Monitors");
        }

        public IActionResult Remove(Guid id)
        {
            List<Guid> ids = HttpContext.Session.GetObject<List<Guid>>("CompareMonitors") ?? new();

            ids.Remove(id);

            HttpContext.Session.SetObject("CompareMonitors", ids);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            IList<Guid> ids = HttpContext.Session.GetObject<List<Guid>>("CompareMonitors") ?? new();
            
            CompareDTO compareDTO = await monitorService.GetMonitorComparisonAsync(ids);

            CompareViewModel viewModel = mapper.Map<CompareViewModel>(compareDTO);

            return View(viewModel);
        }
    }
}
