namespace MonitorLab.Data.Seed.DTOS
{
    public class MonitorSeedDTO
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Resolution { get; set; } = null!;
        public string PanelType { get; set; } = null!;
        public double ScreenSizeInches { get; set; }
        public int RefreshRateHz { get; set; }
        public double ResponseTimeMs { get; set; }
        public int BrightnessNits { get; set; }
        public string ContrastRatio { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int ReleaseYear { get; set; }
    }
}
