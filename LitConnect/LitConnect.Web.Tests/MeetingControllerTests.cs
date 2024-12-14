using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using LitConnect.Web.Controllers;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class MeetingControllerTests : IDisposable
{
    private Mock<IMeetingService> meetingServiceMock = null!;
    private Mock<IBookService> bookServiceMock = null!;
    private Mock<IMeetingMapper> meetingMapperMock = null!;
    private MeetingController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        meetingServiceMock = new Mock<IMeetingService>();
        bookServiceMock = new Mock<IBookService>();
        meetingMapperMock = new Mock<IMeetingMapper>();
        controller = new MeetingController(
            meetingServiceMock.Object,
            bookServiceMock.Object,
            meetingMapperMock.Object);
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithMeeting()
    {
        var meetingId = "1";
        var meetingDto = new MeetingDto
        {
            Id = meetingId,
            Title = "Test Meeting",
            Description = "Test Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            BookClubId = "club1",
            BookClubName = "Test Club",
            BookTitle = "Test Book"
        };

        var expectedViewModel = new MeetingDetailsViewModel
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
            .ReturnsAsync(meetingDto);

        meetingMapperMock.Setup(m => m.MapToDetailsViewModel(meetingDto))
            .Returns(expectedViewModel);

        var actionResult = await controller.Details(meetingId);
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
        var model = new MeetingCreateViewModel
        {
            Title = "New Meeting",
            Description = "New Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            BookClubId = "club1",
            BookId = "book1"
        };
        var meetingId = "new_meeting";

        meetingServiceMock.Setup(s => s.CreateAsync(
                model.Title,
                model.Description,
                model.ScheduledDate,
                model.BookClubId,
                model.BookId))
            .ReturnsAsync(meetingId);

        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(meetingId));
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