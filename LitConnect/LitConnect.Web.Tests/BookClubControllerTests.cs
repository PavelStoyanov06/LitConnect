using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.BookClub;
using LitConnect.Web.ViewModels.Discussion;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class BookClubControllerTests : IDisposable
{
    private Mock<IBookClubService> bookClubServiceMock = null!;
    private Mock<IBookService> bookServiceMock = null!;
    private Mock<IDiscussionService> discussionServiceMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private BookClubController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        bookClubServiceMock = new Mock<IBookClubService>();
        bookServiceMock = new Mock<IBookService>();
        discussionServiceMock = new Mock<IDiscussionService>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        var opts = new Mock<IOptions<IdentityOptions>>();
        var passHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var passValidators = new List<IPasswordValidator<ApplicationUser>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            opts.Object,
            passHasher.Object,
            userValidators,
            passValidators,
            keyNormalizer.Object,
            errors.Object,
            services.Object,
            logger.Object);

        controller = new BookClubController(
            bookClubServiceMock.Object,
            bookServiceMock.Object,
            discussionServiceMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public async Task Index_ShouldReturnViewWithBookClubs()
    {
        // Arrange
        var userId = "user1";
        var expectedClubs = new List<BookClubAllViewModel>
        {
            new()
            {
                Id = "1",
                Name = "Club 1",
                Description = "Description 1",
                MembersCount = 1,
                IsUserMember = true
            },
            new()
            {
                Id = "2",
                Name = "Club 2",
                Description = "Description 2",
                MembersCount = 1,
                IsUserMember = false
            }
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        bookClubServiceMock.Setup(s => s.GetAllAsync(userId))
            .ReturnsAsync(expectedClubs);

        // Act
        var actionResult = await controller.Index();
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedClubs));
        });
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithDetails()
    {
        // Arrange
        var clubId = "1";
        var userId = "user1";
        var expectedDetails = new BookClubDetailsViewModel
        {
            Id = clubId,
            Name = "Test Club",
            Description = "Test Description",
            OwnerId = "owner1",
            OwnerName = "Owner Name",
            MembersCount = 1,
            IsUserMember = true,
            IsUserOwner = false,
            IsUserAdmin = false,
            Discussions = new List<DiscussionInListViewModel>(),
            Meetings = new List<MeetingInListViewModel>(),
            Books = new List<BookClubBookViewModel>()
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        bookClubServiceMock.Setup(s => s.GetDetailsAsync(clubId, userId))
            .ReturnsAsync(expectedDetails);

        // Act
        var actionResult = await controller.Details(clubId);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedDetails));
        });
    }

    [Test]
    public async Task Details_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var clubId = "nonexistent";
        var userId = "user1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        bookClubServiceMock.Setup(s => s.GetDetailsAsync(clubId, userId))
            .ReturnsAsync((BookClubDetailsViewModel?)null);

        // Act
        var result = await controller.Details(clubId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                controller?.Dispose();
            }
            isDisposed = true;
        }
    }
}