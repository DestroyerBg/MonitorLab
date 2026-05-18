using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonitorLab.Core.Services;
using MonitorLab.Data.EntityDTOs;
using Moq;
using NUnit.Framework;

namespace MonitorLab.Tests;

[TestFixture]
public class ApplicationUserServiceTests
{
    private Mock<UserManager<IdentityUser>> userManagerMock = null!;
    private Mock<SignInManager<IdentityUser>> signInManagerMock = null!;
    private ApplicationUserService userService = null!;

    [SetUp]
    public void SetUp()
    {
        userManagerMock = CreateUserManagerMock();
        signInManagerMock = CreateSignInManagerMock(userManagerMock.Object);

        userService = new ApplicationUserService(
            signInManagerMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public void CreateBlankLoginViewModel_ShouldReturnEmptyLoginDto()
    {
        LoginDTO result = userService.CreateBlankLoginViewModel();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.Null.Or.Empty);
        Assert.That(result.Password, Is.Null.Or.Empty);
        Assert.That(result.RememberMe, Is.False);
    }

    [Test]
    public async Task LoginUserAsync_ShouldReturnFailedResult_WhenUserDoesNotExist()
    {
        LoginDTO dto = new LoginDTO
        {
            Email = "missing@monitorlab.local",
            Password = "Test123!",
            RememberMe = false
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((IdentityUser?)null);

        SignInResult result = await userService.LoginUserAsync(dto);

        Assert.That(result.Succeeded, Is.False);

        signInManagerMock.Verify(x =>
            x.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()),
            Times.Never);
    }

    [Test]
    public async Task LoginUserAsync_ShouldCallPasswordSignInAsync_WhenUserExists()
    {
        LoginDTO dto = new LoginDTO
        {
            Email = "admin@monitorlab.local",
            Password = "Admin123!",
            RememberMe = true
        };

        IdentityUser user = new IdentityUser
        {
            UserName = "admin@monitorlab.local",
            Email = "admin@monitorlab.local"
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        signInManagerMock
            .Setup(x => x.PasswordSignInAsync(
                user.UserName,
                dto.Password,
                dto.RememberMe,
                true))
            .ReturnsAsync(SignInResult.Success);

        SignInResult result = await userService.LoginUserAsync(dto);

        Assert.That(result.Succeeded, Is.True);

        signInManagerMock.Verify(x =>
            x.PasswordSignInAsync(
                user.UserName,
                dto.Password,
                dto.RememberMe,
                true),
            Times.Once);
    }

    [Test]
    public async Task LoginUserAsync_ShouldReturnFailedResult_WhenPasswordIsInvalid()
    {
        LoginDTO dto = new LoginDTO
        {
            Email = "admin@monitorlab.local",
            Password = "WrongPassword",
            RememberMe = false
        };

        IdentityUser user = new IdentityUser
        {
            UserName = "admin@monitorlab.local",
            Email = "admin@monitorlab.local"
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        signInManagerMock
            .Setup(x => x.PasswordSignInAsync(
                user.UserName,
                dto.Password,
                dto.RememberMe,
                true))
            .ReturnsAsync(SignInResult.Failed);

        SignInResult result = await userService.LoginUserAsync(dto);

        Assert.That(result.Succeeded, Is.False);
    }

    [Test]
    public async Task LogoutUserAsync_ShouldCallSignOutAsync_AndReturnTrue()
    {
        signInManagerMock
            .Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        bool result = await userService.LogoutUserAsync();

        Assert.That(result, Is.True);

        signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }

    private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
    {
        Mock<IUserStore<IdentityUser>> store = new();

        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static Mock<SignInManager<IdentityUser>> CreateSignInManagerMock(
        UserManager<IdentityUser> userManager)
    {
        Mock<IHttpContextAccessor> contextAccessor = new();
        Mock<IUserClaimsPrincipalFactory<IdentityUser>> claimsFactory = new();

        return new Mock<SignInManager<IdentityUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null!,
            null!,
            null!,
            null!);
    }
}