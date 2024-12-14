using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using LitConnect.Web.Controllers;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Comment;
using LitConnect.Web.ViewModels.Discussion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class DiscussionControllerTests : IDisposable
{
    private Mock<IDiscussionService> discussionServiceMock = null!;
    private Mock<IDiscussionMapper> discussionMapperMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private DiscussionController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        discussionServiceMock = new Mock<IDiscussionService>();
        discussionMapperMock = new Mock<IDiscussionMapper>();
        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        controller = new DiscussionController(
            discussionServiceMock.Object,
            discussionMapperMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithDiscussion()
    {
        var discussionId = "discussion1";
        var userId = "user1";
        var discussionDto = new DiscussionDto
        {
            Id = discussionId,
            Title = "Test Discussion",
            Content = "Test Content",
            AuthorName = "Test Author",
            BookClubId = "club1",
            BookClubName = "Test Club",
            CreatedOn = DateTime.UtcNow,
            IsCurrentUserAuthor = true
        };

        var expectedViewModel = new DiscussionDetailsViewModel
        {
            Id = discussionId,
            Title = "Test Discussion",
            Content = "Test Content",
            AuthorName = "Test Author",
            BookClubId = "club1",
            BookClubName = "Test Club",
            CreatedOn = DateTime.UtcNow,
            NewComment = new CommentCreateViewModel { DiscussionId = discussionId }
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.GetDetailsAsync(discussionId, userId))
            .ReturnsAsync(discussionDto);

        discussionMapperMock.Setup(m => m.MapToDetailsViewModel(discussionDto))
            .Returns(expectedViewModel);

        var actionResult = await controller.Details(discussionId);
        var result = actionResult as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(expectedViewModel));
        });

        discussionServiceMock.Verify(s => s.GetDetailsAsync(discussionId, userId), Times.Once);
        discussionMapperMock.Verify(m => m.MapToDetailsViewModel(discussionDto), Times.Once);
    }

    [Test]
    public async Task Create_Post_WithValidModel_ShouldRedirectToDetails()
    {
        var userId = "user1";
        var discussionId = "new_discussion";
        var model = new DiscussionCreateViewModel
        {
            Title = "New Discussion",
            Content = "New Content",
            BookClubId = "club1",
            BookId = "book1"
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.CreateAsync(
                model.Title,
                model.Content,
                model.BookClubId,
                model.BookId,
                userId))
            .ReturnsAsync(discussionId);

        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(discussionId));
        });
    }

    [Test]
    public async Task Delete_WithAuthorizedUser_ShouldRedirectToBookClub()
    {
        var discussionId = "1";
        var userId = "user1";
        var bookClubId = "club1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        discussionServiceMock.Setup(s => s.GetDetailsAsync(discussionId, userId))
            .ReturnsAsync(new DiscussionDto { BookClubId = bookClubId });

        discussionServiceMock.Setup(s => s.CanUserDeleteAsync(discussionId, userId))
            .ReturnsAsync(true);

        var actionResult = await controller.Delete(discussionId);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ControllerName, Is.EqualTo("BookClub"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(bookClubId));
        });

        discussionServiceMock.Verify(s => s.DeleteAsync(discussionId), Times.Once);
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