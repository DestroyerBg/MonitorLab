using MonitorLab.Core.Services;
using MonitorLab.Data.EntityDTOs;
using NUnit.Framework;

namespace MonitorLab.Tests;

[TestFixture]
public class ComparisonScoreServiceTests
{
    private ComparisonScoreService service = null!;

    [SetUp]
    public void SetUp()
    {
        service = new ComparisonScoreService();
    }

    [Test]
    public void ApplyScores_ShouldCalculateCorrectScores()
    {
        CompareDTO dto = new CompareDTO();

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "LG",
            Model = "OLED Test",
            Resolution = "QHD",
            PanelType = "OLED",
            ScreenSizeInches = 27,
            RefreshRateHz = 240,
            ResponseTimeMs = 0.03,
            BrightnessNits = 250
        });

        CompareDTO result = service.ApplyScores(dto);

        MonitorComparisonCardDTO monitor = result.Monitors.First();

        Assert.That(monitor.GamingScore, Is.EqualTo(15));      // 5 + 5 + 5
        Assert.That(monitor.OfficeScore, Is.EqualTo(11));      // 3 + 4 + 4
        Assert.That(monitor.MultimediaScore, Is.EqualTo(10));  // 5 + 2 + 3
        Assert.That(monitor.DesignScore, Is.EqualTo(10));      // 5 + 3 + 2
    }

    [Test]
    public void ApplyScores_ShouldCalculateDifferentScoresForDifferentMonitors()
    {
        CompareDTO dto = new CompareDTO();

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "Samsung",
            Model = "UHD VA",
            Resolution = "UHD",
            PanelType = "VA",
            ScreenSizeInches = 32,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 400
        });

        CompareDTO result = service.ApplyScores(dto);

        MonitorComparisonCardDTO monitor = result.Monitors.First();

        Assert.That(monitor.GamingScore, Is.EqualTo(11));      // 4 + 4 + 3
        Assert.That(monitor.OfficeScore, Is.EqualTo(14));      // 5 + 5 + 4
        Assert.That(monitor.MultimediaScore, Is.EqualTo(13));  // 4 + 4 + 5
        Assert.That(monitor.DesignScore, Is.EqualTo(12));      // 3 + 5 + 4
    }

    [Test]
    public void ApplyRecommendations_ShouldReturnSameDto_WhenNoMonitorsExist()
    {
        CompareDTO dto = new CompareDTO();

        CompareDTO result = service.ApplyRecommendations(dto);

        Assert.That(result, Is.SameAs(dto));
        Assert.That(result.Recommendations.GamingRecommendation, Is.Empty);
        Assert.That(result.Recommendations.OfficeRecommendation, Is.Empty);
        Assert.That(result.Recommendations.MultimediaRecommendation, Is.Empty);
        Assert.That(result.Recommendations.DesignRecommendation, Is.Empty);
    }

    [Test]
    public void ApplyRecommendations_ShouldSelectMonitorWithHighestScores()
    {
        CompareDTO dto = new CompareDTO();

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "LG",
            Model = "Gaming King",
            GamingScore = 15,
            OfficeScore = 8,
            MultimediaScore = 9,
            DesignScore = 10
        });

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "Dell",
            Model = "Office Master",
            GamingScore = 9,
            OfficeScore = 15,
            MultimediaScore = 8,
            DesignScore = 9
        });

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "Asus",
            Model = "Design Pro",
            GamingScore = 10,
            OfficeScore = 10,
            MultimediaScore = 14,
            DesignScore = 15
        });

        CompareDTO result = service.ApplyRecommendations(dto);

        Assert.That(result.Recommendations.GamingRecommendation, Does.Contain("LG"));
        Assert.That(result.Recommendations.GamingRecommendation, Does.Contain("Gaming King"));

        Assert.That(result.Recommendations.OfficeRecommendation, Does.Contain("Dell"));
        Assert.That(result.Recommendations.OfficeRecommendation, Does.Contain("Office Master"));

        Assert.That(result.Recommendations.MultimediaRecommendation, Does.Contain("Asus"));
        Assert.That(result.Recommendations.MultimediaRecommendation, Does.Contain("Design Pro"));

        Assert.That(result.Recommendations.DesignRecommendation, Does.Contain("Asus"));
        Assert.That(result.Recommendations.DesignRecommendation, Does.Contain("Design Pro"));
    }

    [Test]
    public void ApplyScoresAndRecommendations_ShouldWorkTogether()
    {
        CompareDTO dto = new CompareDTO();

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "LG",
            Model = "OLED Gaming",
            Resolution = "QHD",
            PanelType = "OLED",
            ScreenSizeInches = 27,
            RefreshRateHz = 240,
            ResponseTimeMs = 0.03,
            BrightnessNits = 250
        });

        dto.Monitors.Add(new MonitorComparisonCardDTO
        {
            Brand = "Samsung",
            Model = "Office UHD",
            Resolution = "UHD",
            PanelType = "VA",
            ScreenSizeInches = 32,
            RefreshRateHz = 144,
            ResponseTimeMs = 4,
            BrightnessNits = 400
        });

        service.ApplyScores(dto);
        CompareDTO result = service.ApplyRecommendations(dto);

        Assert.That(result.Recommendations.GamingRecommendation, Does.Contain("LG"));
        Assert.That(result.Recommendations.OfficeRecommendation, Does.Contain("Samsung"));
        Assert.That(result.Recommendations.MultimediaRecommendation, Does.Contain("Samsung"));
        Assert.That(result.Recommendations.DesignRecommendation, Does.Contain("Samsung"));
    }
}