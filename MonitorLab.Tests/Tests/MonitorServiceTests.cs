using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MonitorLab.Core.Services;
using MonitorLab.Data;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Data.Enums;
using MonitorLab.Data.Models;
using MonitorLab.Web.MapperProfiles;
using NUnit.Framework;
using static MonitorLab.Data.Common.DatabaseConstants;
using Monitor = MonitorLab.Data.Models.Monitor;
using MonitorPort = MonitorLab.Data.Models.MonitorPort;
using Port = MonitorLab.Data.Models.Port;
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

    [Test]
    public async Task GetDistinctResolutions_ShouldReturnOnlyExistingDistinctResolutions()
    {
        await SeedMonitors();

        IEnumerable<SelectListItem> result =
            await monitorService.GetDistinctResolutions();

        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(r => r.Text == "QHD" && r.Value == "QHD"), Is.True);
        Assert.That(result.Any(r => r.Text == "UHD" && r.Value == "UHD"), Is.True);
    }

    [Test]
    public async Task GetDistinctPanelTypes_ShouldReturnOnlyExistingDistinctPanelTypes()
    {
        await SeedMonitors();

        IEnumerable<SelectListItem> result =
            await monitorService.GetDistinctPanelTypes();

        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(p => p.Text == "OLED" && p.Value == "OLED"), Is.True);
        Assert.That(result.Any(p => p.Text == "VA" && p.Value == "VA"), Is.True);
    }

    [Test]
    public async Task CreateMonitorAsync_ShouldCreateMonitorAndReturnId()
    {
        MonitorCreateDTO dto = new()
        {
            Brand = "Dell",
            Model = "G2724D",
            Resolution = "QHD",
            PanelType = "IPS",
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 400,
            ContrastRatio = "1000:1",
            Description = "Test monitor",
            ReleaseYear = 2024
        };

        Guid id = await monitorService.CreateMonitorAsync(dto);

        Monitor? monitor = await dbContext.Monitors.FindAsync(id);

        Assert.That(monitor, Is.Not.Null);
        Assert.That(monitor!.Brand, Is.EqualTo("Dell"));
        Assert.That(monitor.Model, Is.EqualTo("G2724D"));
        Assert.That(monitor.Resolution.ToString(), Is.EqualTo("QHD"));
        Assert.That(monitor.PanelType.ToString(), Is.EqualTo("IPS"));
    }

    [Test]
    public async Task UpdateMonitorImageAsync_ShouldUpdateImageUrl_WhenMonitorExists()
    {
        Monitor monitor = new()
        {
            Id = Guid.NewGuid(),
            Brand = "LG",
            Model = "Test",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.IPS,
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "1000:1",
            Description = "Test monitor",
            ImageUrl = null,
            ReleaseYear = 2024
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.SaveChangesAsync();

        await monitorService.UpdateMonitorImageAsync(
            monitor.Id,
            "/images/monitors/test.jpg");

        Monitor? updated = await dbContext.Monitors.FindAsync(monitor.Id);

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.ImageUrl, Is.EqualTo("/images/monitors/test.jpg"));
    }
    [Test]
    public async Task CreateMonitorAsync_ShouldCreateMonitorPorts_WhenPortsAreProvided()
    {
        Guid hdmiId = Guid.NewGuid();
        Guid displayPortId = Guid.NewGuid();

        await dbContext.Ports.AddRangeAsync(
            new Port
            {
                Id = hdmiId,
                Name = "HDMI",
                Version = 2.1
            },
            new Port
            {
                Id = displayPortId,
                Name = "DisplayPort",
                Version = 1.4
            });

        await dbContext.SaveChangesAsync();

        MonitorCreateDTO dto = new()
        {
            Brand = "LG",
            Model = "UltraGear",
            Resolution = "QHD",
            PanelType = "IPS",
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 400,
            ContrastRatio = "1000:1",
            Description = "Gaming monitor",
            ReleaseYear = 2024,

            Ports = new List<MonitorPortCreateDTO>
        {
            new()
            {
                PortId = hdmiId,
                Count = 2,
                IsSelected = true,
            },
            new()
            {
                PortId = displayPortId,
                Count = 1,
                IsSelected = true,
            }
        }
        };

        Guid monitorId =
            await monitorService.CreateMonitorAsync(dto);

        List<MonitorPort> monitorPorts = await dbContext.MonitorPorts
            .Where(mp => mp.MonitorId == monitorId)
            .ToListAsync();

        Assert.That(monitorPorts.Count, Is.EqualTo(2));

        MonitorPort hdmiPort =
            monitorPorts.First(mp => mp.PortId == hdmiId);

        Assert.That(hdmiPort.Count, Is.EqualTo(2));

        MonitorPort displayPort =
            monitorPorts.First(mp => mp.PortId == displayPortId);

        Assert.That(displayPort.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task CreateMonitorAsync_ShouldNotCreateMonitorPorts_WhenNoPortsAreProvided()
    {
        MonitorCreateDTO dto = new()
        {
            Brand = "Samsung",
            Model = "Odyssey G5",
            Resolution = "QHD",
            PanelType = "VA",
            ScreenSizeInches = 27,
            RefreshRateHz = 144,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "2500:1",
            Description = "Curved gaming monitor",
            ReleaseYear = 2024,

            Ports = new List<MonitorPortCreateDTO>()
        };

        Guid monitorId =
            await monitorService.CreateMonitorAsync(dto);

        bool hasPorts = await dbContext.MonitorPorts
            .AnyAsync(mp => mp.MonitorId == monitorId);

        Assert.That(hasPorts, Is.False);
    }

    [Test]
    public async Task UpdateMonitorImageAsync_ShouldNotThrow_WhenMonitorDoesNotExist()
    {
        Assert.DoesNotThrowAsync(async () =>
            await monitorService.UpdateMonitorImageAsync(
                Guid.NewGuid(),
                "/images/monitors/test.jpg"));
    }

    [Test]
    public async Task DeleteMonitorAsync_ShouldReturnNull_WhenMonitorDoesNotExist()
    {
        MonitorDeleteResultDTO result = await monitorService.DeleteMonitorAsync(Guid.NewGuid());

        Assert.That(result.IsDeleted, Is.False);
    }

    [Test]
    public async Task DeleteMonitorAsync_ShouldDeleteMonitor_WhenMonitorExists()
    {
        Monitor monitor = new()
        {
            Id = Guid.NewGuid(),
            Brand = "LG",
            Model = "UltraGear",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.IPS,
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "1000:1",
            Description = "Test monitor",
            ImageUrl = "/images/monitors/lg.jpg",
            ReleaseYear = 2024
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.SaveChangesAsync();

        MonitorDeleteResultDTO result = await monitorService.DeleteMonitorAsync(monitor.Id);

        Monitor? deletedMonitor = await dbContext.Monitors.FindAsync(monitor.Id);

        Assert.That(result.ImageUrl, Is.EqualTo("/images/monitors/lg.jpg"));
        Assert.That(deletedMonitor, Is.Null);
    }

    [Test]
    public async Task DeleteMonitorAsync_ShouldDeleteMonitorPorts_WhenMonitorHasPorts()
    {
        Guid monitorId = Guid.NewGuid();
        Guid hdmiId = Guid.NewGuid();

        Monitor monitor = new()
        {
            Id = monitorId,
            Brand = "Samsung",
            Model = "Odyssey",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.VA,
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "2500:1",
            Description = "Test monitor",
            ImageUrl = "/images/monitors/samsung.jpg",
            ReleaseYear = 2024
        };

        Port hdmi = new()
        {
            Id = hdmiId,
            Name = "HDMI",
            Version = 2.1
        };

        MonitorPort monitorPort = new()
        {
            MonitorId = monitorId,
            Monitor = monitor,
            PortId = hdmiId,
            Port = hdmi,
            Count = 2
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.Ports.AddAsync(hdmi);
        await dbContext.MonitorPorts.AddAsync(monitorPort);
        await dbContext.SaveChangesAsync();

        MonitorDeleteResultDTO result = await monitorService.DeleteMonitorAsync(monitorId);

        bool monitorExists = await dbContext.Monitors.AnyAsync(m => m.Id == monitorId);
        bool monitorPortExists = await dbContext.MonitorPorts.AnyAsync(mp => mp.MonitorId == monitorId);

        Assert.That(result.ImageUrl, Is.EqualTo("/images/monitors/samsung.jpg"));
        Assert.That(monitorExists, Is.False);
        Assert.That(monitorPortExists, Is.False);
    }

    [Test]
    public async Task DeleteMonitorAsync_ShouldReturnNullImageUrl_WhenMonitorHasNoImage()
    {
        Monitor monitor = new()
        {
            Id = Guid.NewGuid(),
            Brand = "AOC",
            Model = "24G4",
            Resolution = ResolutionType.FullHD,
            PanelType = PanelType.IPS,
            ScreenSizeInches = 23.8,
            RefreshRateHz = 180,
            ResponseTimeMs = 1,
            BrightnessNits = 300,
            ContrastRatio = "1000:1",
            Description = "Test monitor",
            ImageUrl = null,
            ReleaseYear = 2024
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.SaveChangesAsync();

        MonitorDeleteResultDTO result = await monitorService.DeleteMonitorAsync(monitor.Id);

        Monitor? deletedMonitor = await dbContext.Monitors.FindAsync(monitor.Id);

        Assert.That(result.ImageUrl, Is.Null);
        Assert.That(deletedMonitor, Is.Null);
    }

    [Test]
    public async Task GetMonitorForEditAsync_ShouldReturnNull_WhenMonitorDoesNotExist()
    {
        MonitorEditDTO? result =
            await monitorService.GetMonitorForEditAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMonitorForEditAsync_ShouldReturnCorrectMonitorData_WhenMonitorExists()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        MonitorEditDTO? result =
            await monitorService.GetMonitorForEditAsync(monitorId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(monitorId));
        Assert.That(result.Brand, Is.EqualTo("Asus"));
        Assert.That(result.Model, Is.EqualTo("ROG Swift OLED PG27AQDM"));
        Assert.That(result.Resolution, Is.EqualTo("QHD"));
        Assert.That(result.PanelType, Is.EqualTo("OLED"));
        Assert.That(result.RefreshRateHz, Is.EqualTo(240));
    }

    [Test]
    public async Task GetMonitorForEditAsync_ShouldReturnSelectedPortsWithCorrectCounts()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        MonitorEditDTO? result =
            await monitorService.GetMonitorForEditAsync(monitorId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Ports.Count, Is.EqualTo(2));

        MonitorPortCreateDTO hdmi =
            result.Ports.First(p => p.Name == "HDMI");

        Assert.That(hdmi.IsSelected, Is.True);
        Assert.That(hdmi.Count, Is.EqualTo(2));

        MonitorPortCreateDTO displayPort =
            result.Ports.First(p => p.Name == "DisplayPort");

        Assert.That(displayPort.IsSelected, Is.True);
        Assert.That(displayPort.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetMonitorForEditAsync_ShouldReturnAllPorts()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        MonitorEditDTO? result =
            await monitorService.GetMonitorForEditAsync(monitorId);

        Assert.That(result!.Ports.Count, Is.EqualTo(2));
        Assert.That(result!.Ports.Any(p => p.Name == "HDMI"), Is.True);
        Assert.That(result!.Ports.Any(p => p.Name == "DisplayPort"), Is.True);
    }

    [Test]
    public async Task GetMonitorForEditAsync_ShouldMarkOnlySelectedPorts()
    {
        Guid monitorId = Guid.NewGuid();

        Guid hdmiId = Guid.NewGuid();
        Guid displayPortId = Guid.NewGuid();
        Guid usbCId = Guid.NewGuid();

        Monitor monitor = new()
        {
            Id = monitorId,
            Brand = "LG",
            Model = "UltraGear",
            Resolution = ResolutionType.QHD,
            PanelType = PanelType.IPS,
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "1000:1",
            Description = "Test monitor",
            ImageUrl = "/images/monitors/lg.jpg",
            ReleaseYear = 2024
        };

        Port hdmi = new()
        {
            Id = hdmiId,
            Name = "HDMI",
            Version = 2.1
        };

        Port displayPort = new()
        {
            Id = displayPortId,
            Name = "DisplayPort",
            Version = 1.4
        };

        Port usbC = new()
        {
            Id = usbCId,
            Name = "USB-C",
            Version = 3.2
        };

        MonitorPort monitorHdmi = new()
        {
            MonitorId = monitorId,
            Monitor = monitor,
            PortId = hdmiId,
            Port = hdmi,
            Count = 2
        };

        await dbContext.Monitors.AddAsync(monitor);
        await dbContext.Ports.AddRangeAsync(hdmi, displayPort, usbC);
        await dbContext.MonitorPorts.AddAsync(monitorHdmi);

        await dbContext.SaveChangesAsync();

        MonitorEditDTO? result =
            await monitorService.GetMonitorForEditAsync(monitorId);

        Assert.That(result, Is.Not.Null);

        Assert.That(result!.Ports.Count, Is.EqualTo(3));

        MonitorPortCreateDTO hdmiPort =
            result.Ports.First(p => p.PortId == hdmiId);

        Assert.That(hdmiPort.IsSelected, Is.True);
        Assert.That(hdmiPort.Count, Is.EqualTo(2));

        MonitorPortCreateDTO displayPortPort =
            result.Ports.First(p => p.PortId == displayPortId);

        Assert.That(displayPortPort.IsSelected, Is.False);
        Assert.That(displayPortPort.Count, Is.EqualTo(1));

        MonitorPortCreateDTO usbCPort =
            result.Ports.First(p => p.PortId == usbCId);

        Assert.That(usbCPort.IsSelected, Is.False);
        Assert.That(usbCPort.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task EditMonitorAsync_ShouldReturnFalse_WhenMonitorDoesNotExist()
    {
        MonitorEditDTO dto = new()
        {
            Id = Guid.NewGuid(),
            Brand = "Missing",
            Model = "Missing",
            Resolution = "QHD",
            PanelType = "IPS",
            ScreenSizeInches = 27,
            RefreshRateHz = 165,
            ResponseTimeMs = 1,
            BrightnessNits = 350,
            ContrastRatio = "1000:1",
            Description = "Missing monitor",
            ReleaseYear = 2024
        };

        bool result =
            await monitorService.EditMonitorAsync(dto);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task EditMonitorAsync_ShouldUpdateMonitorProperties_WhenMonitorExists()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        MonitorEditDTO dto = new()
        {
            Id = monitorId,
            Brand = "Updated LG",
            Model = "Updated Model",
            Resolution = "UHD",
            PanelType = "VA",
            ScreenSizeInches = 32,
            RefreshRateHz = 144,
            ResponseTimeMs = 4,
            BrightnessNits = 500,
            ContrastRatio = "3000:1",
            Description = "Updated description",
            ReleaseYear = 2025,
            Ports = new List<MonitorPortCreateDTO>()
        };

        bool result =
            await monitorService.EditMonitorAsync(dto);

        Monitor? updated =
            await dbContext.Monitors.FindAsync(monitorId);

        Assert.That(result, Is.True);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Brand, Is.EqualTo("Updated LG"));
        Assert.That(updated.Model, Is.EqualTo("Updated Model"));
        Assert.That(updated.Resolution.ToString(), Is.EqualTo("UHD"));
        Assert.That(updated.PanelType.ToString(), Is.EqualTo("VA"));
        Assert.That(updated.ScreenSizeInches, Is.EqualTo(32));
        Assert.That(updated.RefreshRateHz, Is.EqualTo(144));
        Assert.That(updated.ResponseTimeMs, Is.EqualTo(4));
        Assert.That(updated.BrightnessNits, Is.EqualTo(500));
        Assert.That(updated.ContrastRatio, Is.EqualTo("3000:1"));
        Assert.That(updated.Description, Is.EqualTo("Updated description"));
        Assert.That(updated.ReleaseYear, Is.EqualTo(2025));
    }

    [Test]
    public async Task EditMonitorAsync_ShouldReplaceMonitorPorts()
    {
        Guid monitorId = await SeedMonitorWithPorts();

        Guid usbCId = Guid.NewGuid();

        Port usbC = new()
        {
            Id = usbCId,
            Name = "USB-C",
            Version = 3.2
        };

        await dbContext.Ports.AddAsync(usbC);
        await dbContext.SaveChangesAsync();

        MonitorEditDTO dto = new()
        {
            Id = monitorId,
            Brand = "Asus",
            Model = "ROG Swift OLED PG27AQDM",
            Resolution = "QHD",
            PanelType = "OLED",
            ScreenSizeInches = 26.5,
            RefreshRateHz = 240,
            ResponseTimeMs = 0.03,
            BrightnessNits = 240,
            ContrastRatio = "1500000:1",
            Description = "OLED gaming monitor",
            ReleaseYear = 2023,
            Ports = new List<MonitorPortCreateDTO>
        {
            new()
            {
                PortId = usbCId,
                Count = 1,
                IsSelected = true
            }
        }
        };

        bool result =
            await monitorService.EditMonitorAsync(dto);

        List<MonitorPort> monitorPorts =
            await dbContext.MonitorPorts
                .Where(mp => mp.MonitorId == monitorId)
                .ToListAsync();

        Assert.That(result, Is.True);
        Assert.That(monitorPorts.Count, Is.EqualTo(1));
        Assert.That(monitorPorts.First().PortId, Is.EqualTo(usbCId));
        Assert.That(monitorPorts.First().Count, Is.EqualTo(1));
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
