using MonitorLab.Web.Models.MonitorViewModels;

namespace MonitorLab.Web.Models.CompareViewModels
{
    public class MonitorComparisonCardViewModel
    {
        public Guid Id { get; set; }

        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;

        public string Resolution { get; set; } = null!;
        public string PanelType { get; set; } = null!;

        public double ScreenSizeInches { get; set; }

        public int RefreshRateHz { get; set; }

        public double ResponseTimeMs { get; set; }

        public int BrightnessNits { get; set; }

        public string ContrastRatio { get; set; } = null!;

        public IEnumerable<MonitorPortDetailsViewModel> Ports { get; set; }
            = new List<MonitorPortDetailsViewModel>();

        public int GamingScore { get; set; }

        public int OfficeScore { get; set; }

        public int MultimediaScore { get; set; }

        public int DesignScore { get; set; }

        public string GetResolutionPixelsRatio(string resolution)
        {
            switch (resolution)
            {
                case "FullHD":
                    return "1920x1080";
                case "QHD":
                    return "2560x1440";
                case "UHD":
                    return "3840x2160";
                case "UWQHD":
                    return "3440x1440";
                default:
                    return string.Empty;
            }
        }
    }
}