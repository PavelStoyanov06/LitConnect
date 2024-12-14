using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class CommentControllerTests : IDisposable
{
    private Mock<ICommentService> commentServiceMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private CommentController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        commentServiceMock = new Mock<ICommentService>();
        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        controller = new CommentController(commentServiceMock.Object, userManagerMock.Object);
    }

    [Test]
    public async Task Create_WithValidModel_ShouldRedirectToDiscussion()
    {
        var userId = "user1";
        var model = new CommentCreateViewModel
        {
            Content = "Test Comment",
            DiscussionId = "discussion1"
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        commentServiceMock.Setup(s => s.CreateAsync(model.Content, model.DiscussionId, userId))
            .ReturnsAsync("comment1");

        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ControllerName, Is.EqualTo("Discussion"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(model.DiscussionId));
        });
    }

    [Test]
    public async Task Delete_WhenAuthorized_ShouldRedirectToDiscussion()
    {
        var commentId = "comment1";
        var discussionId = "discussion1";
        var userId = "user1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        commentServiceMock.Setup(s => s.IsUserAuthorAsync(commentId, userId))
            .ReturnsAsync(true);

        var actionResult = await controller.Delete(commentId, discussionId);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ControllerName, Is.EqualTo("Discussion"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(discussionId));
        });
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