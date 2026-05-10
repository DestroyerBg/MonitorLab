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

    [Test]
    public void AutoMapperConfiguration_ShouldBeValid()
    {
        MapperConfiguration config = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(MonitorProfiles).Assembly),
            NullLoggerFactory.Instance);

        config.AssertConfigurationIsValid();
    }

    [Test]
    public async Task GetMonitorDetailsAsync_ShouldReturnNull_WhenMonitorDoesNotExist()
    {
        Guid nonExistingId = Guid.NewGuid();

        MonitorDetailsDTO? result = await monitorService.GetMonitorDetailsAsync(nonExistingId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMonitorDetailsAsync_ShouldReturnCorrectMonitorDetails_WhenMonitorExists()
    {
        Guid monitorId = Guid.NewGuid();

        Monitor monitor = new Monitor
        {
            Id = monitorId,
            Brand = "LG",
            Model = "24GS60F",
            Resolution = ResolutionType.FullHD,
            PanelType = PanelType.IPS,
            ScreenSizeInches = 23.8,
            RefreshRateHz = 180,
            ResponseTimeMs = 1,
            BrightnessNits = 300,
            ContrastRatio = "1000:1",
            Description = "Gaming monitor",
            ImageUrl = "/images/monitors/lg.jpg",
            ReleaseYear = 2024
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.SaveChangesAsync();

        MonitorDetailsDTO? result = await monitorService.GetMonitorDetailsAsync(monitorId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(monitorId));
        Assert.That(result.Brand, Is.EqualTo("LG"));
        Assert.That(result.Model, Is.EqualTo("24GS60F"));
        Assert.That(result.Resolution, Is.EqualTo(ResolutionType.FullHD.ToString()));
        Assert.That(result.PanelType, Is.EqualTo(PanelType.IPS.ToString()));
        Assert.That(result.ScreenSizeInches, Is.EqualTo(23.8));
        Assert.That(result.RefreshRateHz, Is.EqualTo(180));
        Assert.That(result.ResponseTimeMs, Is.EqualTo(1));
        Assert.That(result.BrightnessNits, Is.EqualTo(300));
        Assert.That(result.ContrastRatio, Is.EqualTo("1000:1"));
        Assert.That(result.Description, Is.EqualTo("Gaming monitor"));
        Assert.That(result.ImageUrl, Is.EqualTo("/images/monitors/lg.jpg"));
        Assert.That(result.ReleaseYear, Is.EqualTo(2024));
    }

    [Test]
    public async Task GetMonitorDetailsAsync_ShouldReturnPortsWithCorrectCount_WhenMonitorHasPorts()
    {
        Guid monitorId = Guid.NewGuid();
        Guid hdmiId = Guid.NewGuid();
        Guid displayPortId = Guid.NewGuid();

        Monitor monitor = new Monitor
        {
            Id = monitorId,
            Brand = "Samsung",
            Model = "Odyssey G5",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.VA,
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "2500:1",
            Description = "Curved gaming monitor",
            ImageUrl = "/images/monitors/samsung.jpg",
            ReleaseYear = 2023
        };

        Port hdmi = new Port
        {
            Id = hdmiId,
            Name = "HDMI",
            Version = 2.0
        };

        Port displayPort = new Port
        {
            Id = displayPortId,
            Name = "DisplayPort",
            Version = 1.4
        };

        MonitorPort monitorHdmi = new MonitorPort
        {
            MonitorId = monitorId,
            Monitor = monitor,
            PortId = hdmiId,
            Port = hdmi,
            Count = 2
        };

        MonitorPort monitorDisplayPort = new MonitorPort
        {
            MonitorId = monitorId,
            Monitor = monitor,
            PortId = displayPortId,
            Port = displayPort,
            Count = 1
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.Ports.AddRangeAsync(hdmi, displayPort);
        await dbContext.MonitorPorts.AddRangeAsync(monitorHdmi, monitorDisplayPort);
        await dbContext.SaveChangesAsync();

        MonitorDetailsDTO? result = await monitorService.GetMonitorDetailsAsync(monitorId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Ports.Count(), Is.EqualTo(2));

        MonitorPortDetailsDTO hdmiResult = result.Ports.First(p => p.Name == "HDMI");
        Assert.That(hdmiResult.Version, Is.EqualTo(2.0));
        Assert.That(hdmiResult.Count, Is.EqualTo(2));

        MonitorPortDetailsDTO displayPortResult = result.Ports.First(p => p.Name == "DisplayPort");
        Assert.That(displayPortResult.Version, Is.EqualTo(1.4));
        Assert.That(displayPortResult.Count, Is.EqualTo(1));
    }
}
