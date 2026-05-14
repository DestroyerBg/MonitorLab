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

        MonitorCatalogDTO? result = await monitorService.GetMonitorCatalogAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetMonitorCatalogAsync_ShouldReturnEmptyCollection_WhenNoMonitorsExist()
    {
        MonitorCatalogDTO? result = await monitorService.GetMonitorCatalogAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors, Is.Not.Null);
        Assert.That(result.Monitors.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetMonitorCatalogAsync_ShouldPopulateFilterCollections()
    {
        await dbContext.Monitors.AddRangeAsync(
            new Monitor
            {
                Id = Guid.NewGuid(),
                Brand = "LG",
                Model = "UltraGear",
                Resolution = ResolutionType.QHD,
                PanelType = PanelType.OLED,
                ScreenSizeInches = 27,
                RefreshRateHz = 240,
                ResponseTimeMs = 0.03,
                BrightnessNits = 250,
                ContrastRatio = "1000000:1",
                Description = "Test",
                ImageUrl = "/images/lg.jpg",
                ReleaseYear = 2024
            },
            new Monitor
            {
                Id = Guid.NewGuid(),
                Brand = "Samsung",
                Model = "Odyssey",
                Resolution = ResolutionType.UHD,
                PanelType = PanelType.VA,
                ScreenSizeInches = 32,
                RefreshRateHz = 144,
                ResponseTimeMs = 1,
                BrightnessNits = 350,
                ContrastRatio = "2500:1",
                Description = "Test",
                ImageUrl = "/images/samsung.jpg",
                ReleaseYear = 2023
            });

        await dbContext.SaveChangesAsync();

        MonitorCatalogDTO? result = await monitorService.GetMonitorCatalogAsync();

        Assert.That(result, Is.Not.Null);

        Assert.That(result!.Brands.Count(), Is.EqualTo(2));
        Assert.That(result.Resolutions.Count(), Is.EqualTo(2));
        Assert.That(result.PanelTypes.Count(), Is.EqualTo(2));

        Assert.That(result.Brands, Does.Contain("LG"));
        Assert.That(result.Brands, Does.Contain("Samsung"));

        Assert.That(result.Resolutions, Does.Contain("QHD"));
        Assert.That(result.Resolutions, Does.Contain("UHD"));

        Assert.That(result.PanelTypes, Does.Contain("OLED"));
        Assert.That(result.PanelTypes, Does.Contain("VA"));
    }

    [Test]
    public async Task FilterMonitorsAsync_ShouldFilterBySearchTerm()
    {
        await SeedMonitors();

        IEnumerable<MonitorCardDto> result =
            await monitorService.GetMonitorCatalogAsync(
                "Samsung",
                null,
                null,
                null,
                null);

        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Brand, Is.EqualTo("Samsung"));
    }

    [Test]
    public async Task FilterMonitorsAsync_ShouldFilterByBrand()
    {
        await SeedMonitors();

        IEnumerable<MonitorCardDto> result =
            await monitorService.GetMonitorCatalogAsync(
                null,
                "LG",
                null,
                null,
                null);

        Assert.That(result.All(m => m.Brand == "LG"), Is.True);
    }

    [Test]
    public async Task FilterMonitorsAsync_ShouldFilterByResolution()
    {
        await SeedMonitors();

        IEnumerable<MonitorCardDto> result =
            await monitorService.GetMonitorCatalogAsync(
                null,
                null,
                "QHD",
                null,
                null);

        Assert.That(result.All(m => m.Resolution == "QHD"), Is.True);
    }

    [Test]
    public async Task FilterMonitorsAsync_ShouldFilterByPanelType()
    {
        await SeedMonitors();

        IEnumerable<MonitorCardDto> result =
            await monitorService.GetMonitorCatalogAsync(
                null,
                null,
                null,
                "OLED",
                null);

        Assert.That(result.All(m => m.PanelType == "OLED"), Is.True);
    }

    [Test]
    public async Task FilterMonitorsAsync_ShouldFilterByMinimumRefreshRate()
    {
        await SeedMonitors();

        IEnumerable<MonitorCardDto> result =
            await monitorService.GetMonitorCatalogAsync(
                null,
                null,
                null,
                null,
                200);

        Assert.That(result.All(m => m.RefreshRateHz >= 200), Is.True);
    }

    [Test]
    public async Task FilterMonitorsAsync_ShouldApplyAllFilters()
    {
        await SeedMonitors();

        IEnumerable<MonitorCardDto> result =
            await monitorService.GetMonitorCatalogAsync(
                "LG",
                "LG",
                "QHD",
                "OLED",
                240);

        Assert.That(result.Count(), Is.EqualTo(1));

        MonitorCardDto monitor = result.First();

        Assert.That(monitor.Brand, Is.EqualTo("LG"));
        Assert.That(monitor.Resolution, Is.EqualTo("QHD"));
        Assert.That(monitor.PanelType, Is.EqualTo("OLED"));
        Assert.That(monitor.RefreshRateHz, Is.GreaterThanOrEqualTo(240));
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

        MonitorCatalogDTO? result = await monitorService.GetMonitorCatalogAsync();

        MonitorCardDto? monitor = result?.Monitors.FirstOrDefault();

        Assert.That(monitor, Is.Not.Null);
        Assert.That(monitor!.Id, Is.EqualTo(monitorId));
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

    [Test]
    public async Task GetMonitorComparisonAsync_ShouldReturnOnlyExistingMonitors()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        IList<Guid> ids = new List<Guid>
    {
        monitorId,
        Guid.NewGuid()
    };

        CompareDTO result = await monitorService.GetMonitorComparisonAsync(ids);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors.Count, Is.EqualTo(1));
        Assert.That(result.Monitors.First().Id, Is.EqualTo(monitorId));
    }

    [Test]
    public async Task GetMonitorComparisonAsync_ShouldReturnEmptyCollection_WhenIdsAreInvalid()
    {
        IList<Guid> ids = new List<Guid>
    {
        Guid.NewGuid(),
        Guid.NewGuid()
    };

        CompareDTO result = await monitorService.GetMonitorComparisonAsync(ids);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetMonitorComparisonAsync_ShouldReturnMultipleSelectedMonitors()
    {
        Guid firstMonitorId = await SeedMonitorWithPorts();

        Monitor secondMonitor = new Monitor
        {
            Id = Guid.NewGuid(),
            Brand = "LG",
            Model = "UltraGear",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.OLED,
            ScreenSizeInches = 27,
            RefreshRateHz = 240,
            ResponseTimeMs = 0.03,
            BrightnessNits = 250,
            ContrastRatio = "1000000:1",
            Description = "Test",
            ImageUrl = "/images/lg.jpg",
            ReleaseYear = 2024
        };

        await dbContext.Monitors.AddAsync(secondMonitor);
        await dbContext.SaveChangesAsync();

        CompareDTO result = await monitorService.GetMonitorComparisonAsync(
            new List<Guid> { firstMonitorId, secondMonitor.Id });

        Assert.That(result.Monitors.Count, Is.EqualTo(2));
        Assert.That(result.Monitors.Any(m => m.Id == firstMonitorId), Is.True);
        Assert.That(result.Monitors.Any(m => m.Id == secondMonitor.Id), Is.True);
    }

    [Test]
    public async Task GetMonitorComparisonAsync_ShouldMapPortsWithCorrectCount()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        CompareDTO result = await monitorService.GetMonitorComparisonAsync(new List<Guid> { monitorId });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Monitors.Count, Is.EqualTo(1));

        MonitorComparisonCardDTO monitor = result.Monitors.First();

        Assert.That(monitor.Ports.Count(), Is.EqualTo(2));

        MonitorPortDetailsDTO hdmi = monitor.Ports.First(p => p.Name == "HDMI");
        Assert.That(hdmi.Version, Is.EqualTo(2.0));
        Assert.That(hdmi.Count, Is.EqualTo(2));

        MonitorPortDetailsDTO displayPort = monitor.Ports.First(p => p.Name == "DisplayPort");
        Assert.That(displayPort.Version, Is.EqualTo(1.4));
        Assert.That(displayPort.Count, Is.EqualTo(1));
    }

    private async Task SeedMonitors()
    {
        await dbContext.Monitors.AddRangeAsync(
            new Monitor
            {
                Id = Guid.NewGuid(),
                Brand = "LG",
                Model = "UltraGear",
                Resolution = ResolutionType.QHD,
                PanelType = PanelType.OLED,
                ScreenSizeInches = 27,
                RefreshRateHz = 240,
                ResponseTimeMs = 0.03,
                BrightnessNits = 250,
                ContrastRatio = "1000000:1",
                Description = "Test",
                ImageUrl = "/images/lg.jpg",
                ReleaseYear = 2024
            },
            new Monitor
            {
                Id = Guid.NewGuid(),
                Brand = "Samsung",
                Model = "Odyssey",
                Resolution = ResolutionType.UHD,
                PanelType = PanelType.VA,
                ScreenSizeInches = 32,
                RefreshRateHz = 144,
                ResponseTimeMs = 1,
                BrightnessNits = 350,
                ContrastRatio = "2500:1",
                Description = "Test",
                ImageUrl = "/images/samsung.jpg",
                ReleaseYear = 2023
            });

        await dbContext.SaveChangesAsync();
    }

    private async Task<Guid> SeedMonitorWithPorts()
    {
        Guid monitorId = Guid.NewGuid();
        Guid hdmiId = Guid.NewGuid();
        Guid displayPortId = Guid.NewGuid();

        Monitor monitor = new Monitor
        {
            Id = monitorId,
            Brand = "Asus",
            Model = "ROG Swift OLED PG27AQDM",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.OLED,
            ScreenSizeInches = 26.5,
            RefreshRateHz = 240,
            ResponseTimeMs = 0.03,
            BrightnessNits = 240,
            ContrastRatio = "1500000:1",
            Description = "OLED gaming monitor",
            ImageUrl = "/images/monitors/asus.jpg",
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

        return monitorId;
    }
}
