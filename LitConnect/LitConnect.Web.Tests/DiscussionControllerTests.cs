using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.Comment;
using LitConnect.Web.ViewModels.Discussion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class DiscussionControllerTests : IDisposable
{
    private Mock<IDiscussionService> discussionServiceMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private DiscussionController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
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

        controller = new DiscussionController(
            discussionServiceMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public async Task Create_Get_ShouldReturnView()
    {
        // Arrange
        var bookClubId = "club1";

        // Act
        var actionResult = await controller.Create(bookClubId);
        var result = actionResult as ViewResult;
        var model = result?.Model as DiscussionCreateViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model!.BookClubId, Is.EqualTo(bookClubId));
        });
    }

    [Test]
    public async Task Create_Post_WithValidModel_ShouldRedirectToDetails()
    {
        // Arrange
        var userId = "user1";
        var discussionId = "discussion1";
        var model = new DiscussionCreateViewModel
        {
            Title = "Test Discussion",
            Content = "Test Content",
            BookClubId = "club1",
            BookId = "book1"
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.CreateAsync(model, userId))
            .ReturnsAsync(discussionId);

        // Act
        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(discussionId));
        });
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithDiscussion()
    {
        // Arrange
        var discussionId = "discussion1";
        var userId = "user1";
        var expectedDiscussion = new DiscussionDetailsViewModel
        {
            Id = discussionId,
            Title = "Test Discussion",
            Content = "Test Content",
            AuthorName = "Test Author",
            BookClubId = "club1",
            BookClubName = "Test Club",
            CreatedOn = DateTime.UtcNow,
            IsCurrentUserAuthor = true,
            IsCurrentUserAdmin = false,
            IsCurrentUserOwner = false,
            Comments = new List<CommentViewModel>(),
            NewComment = new CommentCreateViewModel { DiscussionId = discussionId }
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.GetDetailsAsync(discussionId, userId))
            .ReturnsAsync(expectedDiscussion);

        // Act
        var actionResult = await controller.Details(discussionId);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedDiscussion));
        });
    }

    [Test]
    public async Task Delete_WithAuthorizedUser_ShouldRedirectToBookClub()
    {
        // Arrange
        var discussionId = "discussion1";
        var userId = "user1";
        var bookClubId = "club1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.GetDetailsAsync(discussionId, userId))
            .ReturnsAsync(new DiscussionDetailsViewModel
            {
                Id = discussionId,
                BookClubId = bookClubId
            });

        discussionServiceMock.Setup(s => s.CanUserDeleteAsync(discussionId, userId))
            .ReturnsAsync(true);

        // Act
        var actionResult = await controller.Delete(discussionId);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ControllerName, Is.EqualTo("BookClub"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(bookClubId));
        });
    }

    [Test]
    public async Task Delete_WithUnauthorizedUser_ShouldReturnForbid()
    {
        // Arrange
        var discussionId = "discussion1";
        var userId = "user1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.GetDetailsAsync(discussionId, userId))
            .ReturnsAsync(new DiscussionDetailsViewModel());

        discussionServiceMock.Setup(s => s.CanUserDeleteAsync(discussionId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await controller.Delete(discussionId);

        // Assert
        Assert.That(result, Is.InstanceOf<ForbidResult>());
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