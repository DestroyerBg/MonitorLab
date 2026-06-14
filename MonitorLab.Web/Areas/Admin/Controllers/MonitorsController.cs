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

            model = await FillPorts(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MonitorCreateViewModel httpModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(httpModel!);
                await FillPorts(httpModel!);
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            string? imageUrl = await monitorService.DeleteMonitorAsync(id);

            if (imageUrl == null)
            {
                TempData["ToastType"] = Error;
                TempData["ToastMessage"] = MonitorNotFound;
                return RedirectToAction(nameof(Dashboard));
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                imageService.DeleteImage(imageUrl);
            }

            TempData["ToastType"] = Success;
            TempData["ToastMessage"] = MonitorDeletedSuccessfully;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            MonitorEditDTO? dto = await monitorService.GetMonitorForEditAsync(id);

            if (dto == null)
            {
                TempData["ToastType"] = Error;
                TempData["ToastMessage"] = "Мониторът не беше намерен.";
                return RedirectToAction(nameof(Dashboard));
            }

            MonitorEditViewModel model = mapper.Map<MonitorEditViewModel>(dto);

            model.Resolutions = await monitorService.GetDistinctResolutions();
            model.PanelTypes = await monitorService.GetDistinctPanelTypes();

            model.Ports = mapper.Map<IList<MonitorPortCreateViewModel>>(
                await monitorService.GetPortsForEditAsync(id));

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(MonitorEditViewModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEditDropdownsAndPorts(inputModel);
                return View(inputModel);
            }

            MonitorEditDTO dto = mapper.Map<MonitorEditDTO>(inputModel);

            dto.Ports = inputModel.Ports
                .Where(p => p.IsSelected)
                .Select(p => new MonitorPortCreateDTO
                {
                    PortId = p.PortId,
                    Count = p.Count,
                    IsSelected = true
                })
                .ToList();

            bool isEdited = await monitorService.EditMonitorAsync(dto);

            if (!isEdited)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Мониторът не беше намерен.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (inputModel.ImageFile != null)
            {
                if (!string.IsNullOrWhiteSpace(inputModel.CurrentImageUrl))
                {
                    imageService.DeleteImage(inputModel.CurrentImageUrl);
                }

                string imageUrl = await imageService.SaveMonitorImageAsync(
                    inputModel.ImageFile,
                    inputModel.Id);

                await monitorService.UpdateMonitorImageAsync(inputModel.Id, imageUrl);
            }

            TempData["ToastType"] = Success;
            TempData["ToastMessage"] = "Мониторът беше редактиран успешно.";

            return RedirectToAction(nameof(Dashboard));
        }

        private async Task PopulateDropdowns(MonitorCreateViewModel model)
        {
            model.Resolutions = await monitorService.GetDistinctResolutions();
            model.PanelTypes = await monitorService.GetDistinctPanelTypes();
        }

        private async Task<MonitorCreateViewModel> FillPorts(MonitorCreateViewModel model)
        {
            model.Ports = mapper.Map<IList<MonitorPortCreateViewModel>>(await monitorService.GetPortsForCreateAsync());
            return model;
        }

        private async Task PopulateEditDropdownsAndPorts(MonitorEditViewModel model)
        {
            model.Resolutions = await monitorService.GetDistinctResolutions();
            model.PanelTypes = await monitorService.GetDistinctPanelTypes();

            model.Ports = mapper.Map<IList<MonitorPortCreateViewModel>>(
                await monitorService.GetPortsForEditAsync(model.Id));
        }
    }
}
