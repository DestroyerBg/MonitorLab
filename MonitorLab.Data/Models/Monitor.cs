using MonitorLab.Data.Enums;
using System.ComponentModel.DataAnnotations;
using MonitorLab.Data.Attributes;
using static MonitorLab.Data.Common.DatabaseConstants.Monitor;
namespace MonitorLab.Data.Models
{
    public class Monitor
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(
            BrandMinLength,
            MinimumLength = BrandMinLength)]
        public string Brand { get; set; } = null!;

        [Required]
        [StringLength(
            ModelMaxLength,
            MinimumLength = ModelMinLength)]
        public string Model { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(ResolutionType))]
        public ResolutionType Resolution { get; set; }

        [Required]
        [EnumDataType(typeof(PanelType))]
        public PanelType PanelType { get; set; }

        [Range(ScreenSizeMin, ScreenSizeMax)]
        public double ScreenSizeInches { get; set; }

        [Range(RefreshRateMin, RefreshRateMax)]
        public int RefreshRateHz { get; set; }

        [Range(ResponseTimeMin, ResponseTimeMax)]
        public double ResponseTimeMs { get; set; }

        [Range(BrightnessMin, BrightnessMax)]
        public int BrightnessNits { get; set; }

        [Required]
        [StringLength(
            ContrastRatioMaxLength,
            MinimumLength = ContrastRatioMaxLength)]
        [ContrastRatio]
        public string ContrastRatio { get; set; } = null!;

        [Required]
        [StringLength(
            DescriptionMaxLength,
            MinimumLength = DescriptionMinLength)]
        public string Description { get; set; } = null!;

        [StringLength(ImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        [Range(ReleaseYearMin, ReleaseYearMax)]
        public int ReleaseYear { get; set; }

        public ICollection<MonitorPort> MonitorPorts { get; set; } = new HashSet<MonitorPort>();
    }
}
