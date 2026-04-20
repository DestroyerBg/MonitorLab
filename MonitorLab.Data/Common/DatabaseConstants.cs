namespace MonitorLab.Data.Common
{
    public static class DatabaseConstants
    {
        public static class Monitor
        {
            // Brand
            public const int BrandMinLength = 2;
            public const int BrandMaxLength = 50;

            // Model
            public const int ModelMinLength = 2;
            public const int ModelMaxLength = 100;

            // Screen Size (inches)
            public const double ScreenSizeMin = 13.0;
            public const double ScreenSizeMax = 60.0;

            // Refresh Rate (Hz)
            public const int RefreshRateMin = 60;
            public const int RefreshRateMax = 500;

            // Response Time (ms)
            public const double ResponseTimeMin = 0.1;
            public const double ResponseTimeMax = 20.0;

            // Brightness (nits)
            public const int BrightnessMin = 100;
            public const int BrightnessMax = 2000;

            // Contrast Ratio
            public const int ContrastRatioMaxLength = 15;

            // Description
            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 2000;

            // Image URL
            public const int ImageUrlMaxLength = 2048;

            // Release Year
            public const int ReleaseYearMin = 1990;
            public const int ReleaseYearMax = 2100;
        }

        public static class Port
        {
            // Name (HDMI, DisplayPort, USB-C и т.н.)
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;

            // Version (пример: 1.4, 2.0, 2.1)
            public const double VersionMin = 1.0;
            public const double VersionMax = 10.0;
        }

        public static class MonitorPort
        {
            public const int CountMin = 1;
            public const int CountMax = 10;
        }
    }
}
