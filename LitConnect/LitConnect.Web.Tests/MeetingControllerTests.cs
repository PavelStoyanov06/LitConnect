using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class MeetingControllerTests : IDisposable
{
    private Mock<IMeetingService> meetingServiceMock = null!;
    private Mock<IBookService> bookServiceMock = null!;
    private MeetingController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        meetingServiceMock = new Mock<IMeetingService>();
        bookServiceMock = new Mock<IBookService>();
        controller = new MeetingController(meetingServiceMock.Object, bookServiceMock.Object);
    }

    [Test]
    public async Task Create_Get_ShouldReturnView()
    {
        // Arrange
        var bookClubId = "club1";

        // Act
        var actionResult = await controller.Create(bookClubId);
        var result = actionResult as ViewResult;
        var model = result?.Model as MeetingCreateViewModel;

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
        var meetingId = "meeting1";
        var model = new MeetingCreateViewModel
        {
            Title = "Test Meeting",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            BookClubId = "club1",
            BookId = "book1"
        };

        meetingServiceMock.Setup(s => s.CreateAsync(model))
            .ReturnsAsync(meetingId);

        // Act
        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(meetingId));
        });
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithMeeting()
    {
        // Arrange
        var meetingId = "meeting1";
        var expectedMeeting = new MeetingDetailsViewModel
        {
            Id = meetingId,
            Title = "Test Meeting",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            BookClubId = "club1",
            BookClubName = "Test Club",
            BookTitle = "Test Book"
        };

        meetingServiceMock.Setup(s => s.GetDetailsAsync(meetingId))
            .ReturnsAsync(expectedMeeting);

        // Act
        var actionResult = await controller.Details(meetingId);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedMeeting));
        });
    }

    [Test]
    public async Task Details_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var meetingId = "nonexistent";
        meetingServiceMock.Setup(s => s.GetDetailsAsync(meetingId))
            .ReturnsAsync((MeetingDetailsViewModel?)null);

        // Act
        var result = await controller.Details(meetingId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_ShouldRedirectToBookClub()
    {
        // Arrange
        var meetingId = "meeting1";
        var bookClubId = "club1";

        // Act
        var actionResult = await controller.Delete(meetingId, bookClubId);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ControllerName, Is.EqualTo("BookClub"));
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(bookClubId));
        });

        meetingServiceMock.Verify(s => s.DeleteAsync(meetingId), Times.Once);
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