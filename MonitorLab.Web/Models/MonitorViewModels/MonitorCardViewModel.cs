namespace MonitorLab.Web.Models.MonitorViewModels
{
    public class MonitorCardViewModel
    {
        public Guid Id { get; set; }

        public string Brand { get; set; } = null!;

        public string Model { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public double ScreenSizeInches { get; set; }

        public string Resolution { get; set; } = null!;

        public string PanelType { get; set; } = null!;

        public int RefreshRateHz { get; set; }

        public double ResponseTimeMs { get; set; }

        public int BrightnessNits { get; set; }
    }
}
