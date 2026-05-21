using Microsoft.AspNetCore.Http;
namespace MonitorLab.Data.EntityDTOs
{
    public class MonitorCreateDTO
    {
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Resolution { get; set; } = null!;
        public string PanelType { get; set; } = null!;
        public double ScreenSizeInches { get; set; }
        public int RefreshRateHz { get; set; }

        public int BrightnessNits { get; set; }

        public string ContrastRatio { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int ReleaseYear { get; set; }

    }
}
