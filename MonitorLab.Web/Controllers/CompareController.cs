using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Infrastructure;
using MonitorLab.Web.Models.CompareViewModels;
using static MonitorLab.Data.Common.TempDataMessages;
namespace MonitorLab.Web.Controllers
{
    public class CompareController(
        IMonitorService monitorService,
        IMapper mapper) : Controller
    {
        public async Task<IActionResult> Add(Guid id, string? returnUrl = null)
        {
            IList<Guid> ids = HttpContext.Session.GetObject<List<Guid>>("CompareMonitors") ?? new();
            MonitorDetailsDTO? monitor = await monitorService.GetMonitorDetailsAsync(id);

            if (monitor == null)
            {
                TempData["ToastType"] = Error;
                TempData["ToastMessage"] = MonitorNotFound;
                return Redirect(returnUrl ?? Url.Action("Index", "Monitors")!);
            }

            if (ids.Contains(id))
            {
                TempData["ToastType"] = Error;
                TempData["ToastMessage"] = MonitorAlreadyAdded(monitor.Brand, monitor.Model);
                return Redirect(returnUrl ?? Url.Action("Index", "Monitors")!);
            }

            if (ids.Count >= 3)
            {
                TempData["ToastType"] = Error;
                TempData["ToastMessage"] = CompareLimitReached;
                return Redirect(returnUrl ?? Url.Action("Index", "Monitors")!);
            }

            ids.Add(id);
            HttpContext.Session.SetObject("CompareMonitors", ids);

            TempData["ToastType"] = Success;
            TempData["ToastMessage"] = MonitorAddedSuccessfully(monitor.Brand, monitor.Model);
            

            return ids.Count == 3 ? RedirectToAction(nameof(Index))
                : Redirect(returnUrl ?? Url.Action("Index", "Monitors")!); 
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
