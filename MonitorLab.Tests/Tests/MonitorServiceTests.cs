using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MonitorLab.Core.Services;
using MonitorLab.Data;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Data.Enums;
using MonitorLab.Data.Models;
using MonitorLab.Web.MapperProfiles;
using NUnit.Framework;
using Monitor = MonitorLab.Data.Models.Monitor;
namespace MonitorLab.Tests;


[TestFixture]
public class MonitorServiceTests
{
    private ApplicationDbContext dbContext = null!;
    private IMapper mapper = null!;
    private MonitorService monitorService = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new ApplicationDbContext(options);
        MapperConfiguration config = new MapperConfiguration(
                            cfg => cfg.AddMaps(typeof(MonitorProfiles).Assembly),
                            NullLoggerFactory.Instance);

        mapper = config.CreateMapper();
        monitorService = new MonitorService(mapper, dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Dispose();
    }

    [Test]
    public async Task GetMonitorCatalogAsync_ShouldReturnAllMonitors()
    {
        await dbContext.Monitors.AddRangeAsync(
            new Monitor
            {
                Id = Guid.NewGuid(),
                Brand = "LG",
                Model = "24GS60F",
                Resolution = ResolutionType.FullHD,
                PanelType = PanelType.IPS,
                ScreenSizeInches = 23.8,
                RefreshRateHz = 180,
                ResponseTimeMs = 1,
                BrightnessNits = 300,
                ContrastRatio = "1000:1",
                Description = "Test description",
                ImageUrl = "/images/monitors/lg.jpg",
                ReleaseYear = 2024
            },
            new Monitor
            {
                Id = Guid.NewGuid(),
                Brand = "Samsung",
                Model = "Odyssey G5",
                Resolution = ResolutionType.QHD,
                PanelType = PanelType.VA,
                ScreenSizeInches = 27,
                RefreshRateHz = 165,
                ResponseTimeMs = 1,
                BrightnessNits = 350,
                ContrastRatio = "2500:1",
                Description = "Test description",
                ImageUrl = "/images/monitors/samsung.jpg",
                ReleaseYear = 2023
            });

        await dbContext.SaveChangesAsync();

        MonitorCatalogDTO result = await monitorService.GetMonitorCatalogAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetMonitorCatalogAsync_ShouldReturnEmptyCollection_WhenNoMonitorsExist()
    {
        MonitorCatalogDTO result = await monitorService.GetMonitorCatalogAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors, Is.Not.Null);
        Assert.That(result.Monitors.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetMonitorCatalogAsync_ShouldMapMonitorPropertiesCorrectly()
    {
        var monitorId = Guid.NewGuid();

        await dbContext.Monitors.AddAsync(new Monitor
        {
            Id = monitorId,
            Brand = "AOC",
            Model = "24G4",
            Resolution = ResolutionType.FullHD,
            PanelType = PanelType.IPS,
            ScreenSizeInches = 23.8,
            RefreshRateHz = 180,
            ResponseTimeMs = 1,
            BrightnessNits = 300,
            ContrastRatio = "1000:1",
            Description = "Gaming monitor",
            ImageUrl = "/images/monitors/aoc.jpg",
            ReleaseYear = 2024
        });

        await dbContext.SaveChangesAsync();

        MonitorCatalogDTO result = await monitorService.GetMonitorCatalogAsync();

        MonitorCardDto monitor = result.Monitors.First();

        Assert.That(monitor.Id, Is.EqualTo(monitorId));
        Assert.That(monitor.Brand, Is.EqualTo("AOC"));
        Assert.That(monitor.Model, Is.EqualTo("24G4"));
        Assert.That(monitor.ScreenSizeInches, Is.EqualTo(23.8));
        Assert.That(monitor.RefreshRateHz, Is.EqualTo(180));
        Assert.That(monitor.ResponseTimeMs, Is.EqualTo(1));
        Assert.That(monitor.BrightnessNits, Is.EqualTo(300));
        Assert.That(monitor.ImageUrl, Is.EqualTo("/images/monitors/aoc.jpg"));
    }
}
