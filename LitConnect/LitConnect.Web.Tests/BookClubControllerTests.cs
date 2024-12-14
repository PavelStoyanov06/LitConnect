using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using LitConnect.Web.Controllers;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
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
    private Mock<IBookClubMapper> bookClubMapperMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private BookClubController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        bookClubServiceMock = new Mock<IBookClubService>();
        bookServiceMock = new Mock<IBookService>();
        bookClubMapperMock = new Mock<IBookClubMapper>();

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
            bookClubMapperMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public async Task Index_ShouldReturnViewWithBookClubs()
    {
        var userId = "user1";
        var bookClubDtos = new List<BookClubDto>
        {
            new()
            {
                Id = "1",
                Name = "Club 1",
                Description = "Description 1",
                OwnerId = "owner1",
                OwnerName = "Owner 1",
                MembersCount = 1,
                IsUserMember = true
            },
            new()
            {
                Id = "2",
                Name = "Club 2",
                Description = "Description 2",
                OwnerId = "owner2",
                OwnerName = "Owner 2",
                MembersCount = 1,
                IsUserMember = false
            }
        };

        var expectedViewModels = new List<BookClubAllViewModel>
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
            .ReturnsAsync(bookClubDtos);

        bookClubMapperMock.Setup(m => m.MapToAllViewModels(bookClubDtos))
            .Returns(expectedViewModels);

        var actionResult = await controller.Index();
        var result = actionResult as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedViewModels));
        });
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithDetails()
    {
        var clubId = "1";
        var userId = "user1";

        var bookClubDto = new BookClubDto
        {
            Id = clubId,
            Name = "Test Club",
            Description = "Test Description",
            OwnerId = "owner1",
            OwnerName = "Owner Name",
            MembersCount = 1,
            IsUserMember = true,
            IsUserOwner = false,
            IsUserAdmin = false
        };

        var expectedViewModel = new BookClubDetailsViewModel
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
            .ReturnsAsync(bookClubDto);

        bookClubMapperMock.Setup(m => m.MapToDetailsViewModel(bookClubDto))
            .Returns(expectedViewModel);

        var actionResult = await controller.Details(clubId);
        var result = actionResult as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedViewModel));
        });
    }

    [Test]
    public async Task Create_Post_WithValidModel_ShouldRedirectToDetails()
    {
        var userId = "user1";
        var model = new BookClubCreateViewModel
        {
            Name = "New Club",
            Description = "New Description"
        };
        var expectedClubId = "new_club";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        bookClubServiceMock.Setup(s => s.CreateAsync(model.Name, model.Description, userId))
            .ReturnsAsync(expectedClubId);

        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(expectedClubId));
        });
    }

    [Test]
    public async Task Join_ShouldRedirectToDetails()
    {
        var clubId = "club1";
        var userId = "user1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        var actionResult = await controller.Join(clubId);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(clubId));
        });

        bookClubServiceMock.Verify(
            s => s.JoinBookClubAsync(clubId, userId),
            Times.Once);
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