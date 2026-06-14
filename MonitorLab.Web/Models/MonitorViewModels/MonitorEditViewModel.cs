using Microsoft.AspNetCore.Mvc.Rendering;
using MonitorLab.Data.Common;
using System.ComponentModel.DataAnnotations;
using static MonitorLab.Data.Common.DatabaseConstants.Monitor;
using static MonitorLab.Data.Common.ErrorMessages.Monitor;

namespace MonitorLab.Web.Models.MonitorViewModels
{
    public class MonitorEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = BrandRequired)]
        [StringLength(BrandMaxLength, MinimumLength = BrandMinLength, ErrorMessage = BrandLength)]
        public string Brand { get; set; } = null!;

        [Required(ErrorMessage = ModelRequired)]
        [StringLength(ModelMaxLength, MinimumLength = ModelMinLength, ErrorMessage = ModelLength)]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = ResolutionRequired)]
        public string Resolution { get; set; } = null!;

        [Required(ErrorMessage = PanelTypeRequired)]
        public string PanelType { get; set; } = null!;

        [Range(ScreenSizeMin, ScreenSizeMax, ErrorMessage = ScreenSizeRange)]
        public double ScreenSizeInches { get; set; }

        [Range(RefreshRateMin, RefreshRateMax, ErrorMessage = RefreshRateRange)]
        public int RefreshRateHz { get; set; }

        [Range(ResponseTimeMin, ResponseTimeMax, ErrorMessage = ResponseTimeRange)]
        public double ResponseTimeMs { get; set; }

        [Range(BrightnessMin, BrightnessMax, ErrorMessage = BrightnessRange)]
        public int BrightnessNits { get; set; }

        [Required(ErrorMessage = ContrastRatioRequired)]
        [StringLength(ContrastRatioMaxLength, ErrorMessage = ContrastRatioLength)]
        public string ContrastRatio { get; set; } = null!;

        [Required(ErrorMessage = DescriptionRequired)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = DescriptionLength)]
        public string Description { get; set; } = null!;

        [Range(ReleaseYearMin, ReleaseYearMax, ErrorMessage = ReleaseYearRange)]
        public int ReleaseYear { get; set; }

        public string? CurrentImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public IEnumerable<SelectListItem> Resolutions { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> PanelTypes { get; set; } = new List<SelectListItem>();

        public IList<MonitorPortCreateViewModel> Ports { get; set; } = new List<MonitorPortCreateViewModel>();
    }
}
