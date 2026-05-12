using Microsoft.AspNetCore.Mvc;
using MonitorLab.Web.Infrastructure;
namespace MonitorLab.Web.Controllers
{
    public class CompareController : Controller
    {
        public IActionResult Add(Guid id)
        {
            List<Guid> ids = HttpContext.Session.GetObject<List<Guid>>("CompareMonitors") ?? new();

            if (!ids.Contains(id) && ids.Count < 3)
            {
                ids.Add(id);
            }

            HttpContext.Session.SetObject("CompareMonitors", ids);

            return RedirectToAction("Index", "Monitors");
        }


    }
}
