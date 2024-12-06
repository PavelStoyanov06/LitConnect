using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class MeetingServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private MeetingService meetingService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectMeetingTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new LitConnectDbContext(this.dbOptions);
        this.meetingService = new MeetingService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }

    [Test]
    public async Task GetBookClubMeetingsAsync_ShouldReturnAllNonDeletedMeetings()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";

        // Act
        var result = await meetingService.GetBookClubMeetingsAsync(bookClubId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectMeeting()
    {
        // Arrange
        await SeedDataAsync();
        var meetingId = "meeting1";

        // Act
        var result = await meetingService.GetDetailsAsync(meetingId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Test Meeting 1"));
        Assert.That(result.BookClubId, Is.EqualTo("bookclub1"));
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewMeeting()
    {
        // Arrange
        await SeedDataAsync();
        var model = new MeetingCreateViewModel
        {
            Title = "New Meeting",
            Description = "New Description",
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            BookClubId = "bookclub1",
            BookId = "book1"
        };

        // Act
        var meetingId = await meetingService.CreateAsync(model);

        // Assert
        var createdMeeting = await dbContext.Meetings.FindAsync(meetingId);
        Assert.That(createdMeeting, Is.Not.Null);
        Assert.That(createdMeeting!.Title, Is.EqualTo(model.Title));
        Assert.That(createdMeeting.BookClubId, Is.EqualTo(model.BookClubId));
    }

    [Test]
    public async Task DeleteAsync_ShouldSoftDeleteMeeting()
    {
        // Arrange
        await SeedDataAsync();
        var meetingId = "meeting1";

        // Act
        await meetingService.DeleteAsync(meetingId);

        // Assert
        var deletedMeeting = await dbContext.Meetings.FindAsync(meetingId);
        Assert.That(deletedMeeting!.IsDeleted, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        await SeedDataAsync();
        var meetingId = "meeting1";

        // Act
        var result = await meetingService.ExistsAsync(meetingId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        await SeedDataAsync();
        var meetingId = "nonexistent";

        // Act
        var result = await meetingService.ExistsAsync(meetingId);

        // Assert
        Assert.That(result, Is.False);
    }

    private async Task SeedDataAsync()
    {
        var bookClub = new BookClub
        {
            Id = "bookclub1",
            Name = "Test Book Club",
            OwnerId = "user1",
            IsDeleted = false
        };

        var book = new Book
        {
            Id = "book1",
            Title = "Test Book",
            Author = "Test Author",
            IsDeleted = false
        };

        var meetings = new[]
        {
            new Meeting
            {
                Id = "meeting1",
                Title = "Test Meeting 1",
                Description = "Description 1",
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                BookClubId = "bookclub1",
                BookId = "book1",
                IsDeleted = false
            },
            new Meeting
            {
                Id = "meeting2",
                Title = "Test Meeting 2",
                Description = "Description 2",
                ScheduledDate = DateTime.UtcNow.AddDays(2),
                BookClubId = "bookclub1",
                IsDeleted = false
            },
            new Meeting
            {
                Id = "meeting3",
                Title = "Deleted Meeting",
                BookClubId = "bookclub1",
                IsDeleted = true
            }
        };

        await dbContext.BookClubs.AddAsync(bookClub);
        await dbContext.Books.AddAsync(book);
        await dbContext.Meetings.AddRangeAsync(meetings);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}