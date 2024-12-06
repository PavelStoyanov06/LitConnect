using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.ReadingList;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class ReadingListControllerTests : IDisposable
{
    private Mock<IReadingListService> readingListServiceMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private ReadingListController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        readingListServiceMock = new Mock<IReadingListService>();

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

        controller = new ReadingListController(
            readingListServiceMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public async Task Index_ShouldReturnViewWithReadingList()
    {
        // Arrange
        var userId = "user1";
        var expectedList = new ReadingListViewModel
        {
            Id = "list1",
            UserId = userId,
            UserName = "Test User",
            Books = new List<ReadingListBookViewModel>(),
            BooksCount = 0
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        readingListServiceMock.Setup(s => s.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedList);

        // Act
        var actionResult = await controller.Index();
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedList));
        });
    }

    [Test]
    public async Task AddBook_ShouldRedirectToIndex()
    {
        // Arrange
        var userId = "user1";
        var bookId = "book1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        // Act
        var actionResult = await controller.AddBook(bookId);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Index"));
        });

        readingListServiceMock.Verify(s => s.AddBookAsync(userId, bookId), Times.Once);
    }

    [Test]
    public async Task RemoveBook_ShouldRedirectToIndex()
    {
        // Arrange
        var userId = "user1";
        var bookId = "book1";

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        // Act
        var actionResult = await controller.RemoveBook(bookId);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Index"));
        });

        readingListServiceMock.Verify(s => s.RemoveBookAsync(userId, bookId), Times.Once);
    }

    [Test]
    public async Task UpdateStatus_ShouldRedirectToIndex()
    {
        // Arrange
        var userId = "user1";
        var bookId = "book1";
        var status = ReadingStatus.Reading;

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        // Act
        var actionResult = await controller.UpdateStatus(bookId, status);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Index"));
        });

        readingListServiceMock.Verify(s => s.UpdateBookStatusAsync(userId, bookId, status), Times.Once);
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