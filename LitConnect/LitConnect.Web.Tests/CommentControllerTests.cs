using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

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
            store.Object, opts.Object, passHasher.Object, userValidators,
            passValidators, keyNormalizer.Object, errors.Object,
            services.Object, logger.Object);

        controller = new CommentController(commentServiceMock.Object, userManagerMock.Object);
    }

    [Test]
    public async Task Create_WithValidModel_ShouldRedirectToDiscussion()
    {
        // Arrange
        var userId = "user1";
        var model = new CommentCreateViewModel
        {
            Content = "Test Comment",
            DiscussionId = "discussion1"
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        // Act
        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        // Assert
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
        // Arrange
        var commentId = "comment1";
        var discussionId = "discussion1";
        var userId = "user1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        commentServiceMock.Setup(s => s.IsUserAuthorAsync(commentId, userId))
            .ReturnsAsync(true);

        // Act
        var actionResult = await controller.Delete(commentId, discussionId);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ControllerName, Is.EqualTo("Discussion"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(discussionId));
        });
    }

    [TearDown]
    public void TearDown() => Dispose();

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