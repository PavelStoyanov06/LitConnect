using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using LitConnect.Web.Controllers;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.ReadingList;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class ReadingListControllerTests : IDisposable
{
    private Mock<IReadingListService> readingListServiceMock = null!;
    private Mock<IReadingListMapper> readingListMapperMock = null!;
    private Mock<UserManager<ApplicationUser>> userManagerMock = null!;
    private ReadingListController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        readingListServiceMock = new Mock<IReadingListService>();
        readingListMapperMock = new Mock<IReadingListMapper>();
        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);

        controller = new ReadingListController(
            readingListServiceMock.Object,
            readingListMapperMock.Object,
            userManagerMock.Object);
    }

    [Test]
    public async Task Index_ShouldReturnViewWithReadingList()
    {
        var userId = "user1";
        var readingListDto = new ReadingListDto
        {
            Id = "list1",
            UserId = userId,
            UserName = "Test User",
            Books = new List<ReadingListBookDto>()
        };

        var expectedViewModel = new ReadingListViewModel
        {
            Id = "list1",
            UserId = userId,
            UserName = "Test User",
            Books = new List<ReadingListBookViewModel>()
        };

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        readingListServiceMock.Setup(s => s.GetByUserIdAsync(userId))
            .ReturnsAsync(readingListDto);

        readingListMapperMock.Setup(m => m.MapToViewModel(readingListDto))
            .Returns(expectedViewModel);

        var actionResult = await controller.Index();
        var result = actionResult as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedViewModel));
        });
    }

    [Test]
    public async Task UpdateStatus_ShouldRedirectToIndex()
    {
        var userId = "user1";
        var bookId = "book1";
        var status = LitConnect.Web.ViewModels.ReadingList.ReadingStatus.Reading;

        userManagerMock.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .Returns(userId);

        var actionResult = await controller.UpdateStatus(bookId, status);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Index"));
        });

        readingListServiceMock.Verify(
            s => s.UpdateBookStatusAsync(userId, bookId, (LitConnect.Services.Models.ReadingStatus)status),
            Times.Once);
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