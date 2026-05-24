using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MonitorLab.Web.Services;
using Moq;
using NUnit.Framework;
using System.Text;

namespace MonitorLab.Tests.Tests;

[TestFixture]
public class ImageServiceTests
{
    private string webRootPath = null!;
    private ImageService imageService = null!;

    [SetUp]
    public void SetUp()
    {
        webRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(webRootPath);

        Mock<IWebHostEnvironment> environmentMock = new();
        environmentMock
            .Setup(e => e.WebRootPath)
            .Returns(webRootPath);

        imageService = new ImageService(environmentMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(webRootPath))
        {
            Directory.Delete(webRootPath, recursive: true);
        }
    }

    [Test]
    public async Task SaveMonitorImageAsync_ShouldSaveFileAndReturnImageUrl()
    {
        Guid monitorId = Guid.NewGuid();

        byte[] content = Encoding.UTF8.GetBytes("fake image content");

        IFormFile file = new FormFile(
            new MemoryStream(content),
            0,
            content.Length,
            "ImageFile",
            "test.jpg");

        string result =
            await imageService.SaveMonitorImageAsync(file, monitorId);

        string expectedUrl = $"/images/monitors/{monitorId}.jpg";
        string expectedPath = Path.Combine(
            webRootPath,
            "images",
            "monitors",
            $"{monitorId}.jpg");

        Assert.That(result, Is.EqualTo(expectedUrl));
        Assert.That(File.Exists(expectedPath), Is.True);
    }

    [Test]
    public async Task SaveMonitorImageAsync_ShouldCreateDirectory_WhenDirectoryDoesNotExist()
    {
        Guid monitorId = Guid.NewGuid();

        IFormFile file = new FormFile(
            new MemoryStream(Encoding.UTF8.GetBytes("test")),
            0,
            4,
            "ImageFile",
            "monitor.png");

        await imageService.SaveMonitorImageAsync(file, monitorId);

        string folderPath = Path.Combine(webRootPath, "images", "monitors");

        Assert.That(Directory.Exists(folderPath), Is.True);
    }

    [Test]
    public async Task DeleteImage_ShouldDeleteExistingImage()
    {
        Guid monitorId = Guid.NewGuid();

        IFormFile file = new FormFile(
            new MemoryStream(Encoding.UTF8.GetBytes("test")),
            0,
            4,
            "ImageFile",
            "monitor.jpg");

        string imageUrl =
            await imageService.SaveMonitorImageAsync(file, monitorId);

        imageService.DeleteImage(imageUrl);

        string expectedPath = Path.Combine(
            webRootPath,
            "images",
            "monitors",
            $"{monitorId}.jpg");

        Assert.That(File.Exists(expectedPath), Is.False);
    }

    [Test]
    public void DeleteImage_ShouldNotThrow_WhenFileDoesNotExist()
    {
        Assert.DoesNotThrow(() =>
            imageService.DeleteImage("/images/monitors/missing.jpg"));
    }
}
