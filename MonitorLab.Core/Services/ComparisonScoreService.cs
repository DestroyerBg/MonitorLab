using MonitorLab.Core.Contracts;
using MonitorLab.Data.EntityDTOs;

namespace MonitorLab.Core.Services
{
    public class ComparisonScoreService : IComparisonScoreService
    {
        public CompareDTO ApplyRecommendations(CompareDTO dto)
        {
            if (!dto.Monitors.Any())
            {
                return dto;
            }

            dto.Recommendations.GamingRecommendation =
                GetRecommendationText(dto.Monitors.MaxBy(m => m.GamingScore), "гейминг");

            dto.Recommendations.OfficeRecommendation =
                GetRecommendationText(dto.Monitors.MaxBy(m => m.OfficeScore), "офис работа");

            dto.Recommendations.MultimediaRecommendation =
                GetRecommendationText(dto.Monitors.MaxBy(m => m.MultimediaScore), "мултимедия");

            dto.Recommendations.DesignRecommendation =
                GetRecommendationText(dto.Monitors.MaxBy(m => m.DesignScore), "дизайн и обработка");

            return dto;
        }

        public CompareDTO ApplyScores(CompareDTO model)
        {
            foreach (MonitorComparisonCardDTO monitor in model.Monitors)
            {
                monitor.GamingScore =
                    GetRefreshRateScore(monitor.RefreshRateHz) +
                    GetResponseTimeScore(monitor.ResponseTimeMs) +
                    GetGamingPanelScore(monitor.PanelType);

                monitor.OfficeScore =
                    GetResolutionScore(monitor.Resolution) +
                    GetScreenSizeScore(monitor.ScreenSizeInches) +
                    GetOfficePanelScore(monitor.PanelType);

                monitor.MultimediaScore =
                    GetPanelContrastScore(monitor.PanelType) +
                    GetBrightnessScore(monitor.BrightnessNits) +
                    GetResolutionScore(monitor.Resolution);

                monitor.DesignScore =
                    GetDesignPanelScore(monitor.PanelType) +
                    GetResolutionScore(monitor.Resolution) +
                    GetBrightnessScore(monitor.BrightnessNits);
            }

            return model;
        }

        private static int GetDesignPanelScore(string panelType)
        {
            return panelType.ToUpper() switch
            {
                "OLED" => 5,
                "IPS" => 5,
                "QLED" => 4,
                "VA" => 3,
                "TN" => 1,
                _ => 3
            };
        }

        private static int GetPanelContrastScore(string panelType)
        {
            return panelType.ToUpper() switch
            {
                "OLED" => 5,
                "VA" => 4,
                "QLED" => 4,
                "IPS" => 3,
                "TN" => 2,
                _ => 3
            };
        }

        private static int GetOfficePanelScore(string panelType)
        {
            return panelType.ToUpper() switch
            {
                "IPS" => 5,
                "OLED" => 4,
                "VA" => 4,
                "TN" => 2,
                _ => 3
            };
        }

        private static int GetGamingPanelScore(string panelType)
        {
            return panelType.ToUpper() switch
            {
                "OLED" => 5,
                "TN" => 4,
                "IPS" => 4,
                "VA" => 3,
                _ => 2
            };
        }

        private static int GetResolutionScore(string resolution)
        {
            return resolution.ToUpper() switch
            {
                "UHD" or "4K" => 5,
                "UWQHD" => 4,
                "QHD" => 3,
                "FULLHD" => 2,
                _ => 1
            };
        }

        private static int GetScreenSizeScore(double screenSize)
        {
            if (screenSize >= 32) return 5;
            if (screenSize >= 27) return 4;
            if (screenSize >= 24) return 3;

            return 2;
        }

        private static int GetBrightnessScore(int brightness)
        {
            if (brightness >= 600) return 5;
            if (brightness >= 400) return 4;
            if (brightness >= 300) return 3;
            if (brightness >= 250) return 2;

            return 1;
        }

        private static int GetResponseTimeScore(double responseTime)
        {
            if (responseTime <= 0.1) return 5;
            if (responseTime <= 1) return 4;
            if (responseTime <= 4) return 3;
            if (responseTime <= 5) return 2;

            return 1;
        }

        private static int GetRefreshRateScore(int refreshRate)
        {
            if (refreshRate >= 240) return 5;
            if (refreshRate >= 165) return 4;
            if (refreshRate >= 144) return 3;
            if (refreshRate >= 100) return 2;

            return 1;
        }

        private static string GetRecommendationText(MonitorComparisonCardDTO? monitor, string usage)       
        {
            if (monitor == null)
            {
                return string.Empty;
            }

            return $"{monitor.Brand} {monitor.Model} е най-подходящ за {usage} според сравнените параметри.";
        }
    }
}
