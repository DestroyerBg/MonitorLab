using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Contracts;
using MonitorLab.Web.Models.MonitorViewModels;
using MonitorLab.Web.Services;
using static MonitorLab.Data.Common.TempDataMessages;
namespace MonitorLab.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MonitorsController(
        IMapper mapper, 
        IMonitorService monitorService,
        IImageService imageService) : Controller
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
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MonitorCreateViewModel httpModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(httpModel!);
                return View(httpModel);
            }

            MonitorCreateDTO dto = mapper.Map<MonitorCreateDTO>(httpModel);

            Guid monitorId = await monitorService.CreateMonitorAsync(dto);

            if (httpModel.ImageFile != null)
            {
                string imageUrl = await imageService.SaveMonitorImageAsync(
                    httpModel.ImageFile,
                    monitorId);

                await monitorService.UpdateMonitorImageAsync(monitorId, imageUrl);
            }

            TempData["ToastType"] = Success;
            TempData["ToastMessage"] = MonitorAddedSuccessfullyIntoDatabase;

            return RedirectToAction(nameof(Dashboard));
        }

        private async Task PopulateDropdowns(MonitorCreateViewModel model)
        {
            model.Resolutions = await monitorService.GetDistinctResolutions();
            model.PanelTypes = await monitorService.GetDistinctPanelTypes();
        }
    }
}
