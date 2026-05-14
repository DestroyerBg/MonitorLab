namespace MonitorLab.Data.EntityDTOs
{
    public class MonitorComparisonCardDTO
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

        public IEnumerable<MonitorPortDetailsDTO> Ports { get; set; }
            = new List<MonitorPortDetailsDTO>();

        public int GamingScore { get; set; }

        public int OfficeScore { get; set; }

        public int MultimediaScore { get; set; }

        public int DesignScore { get; set; }
    }
}
