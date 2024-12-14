using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
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
        await SeedDataAsync();
        var bookClubId = "bookclub1";

        var result = await meetingService.GetBookClubMeetingsAsync(bookClubId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectMeeting()
    {
        await SeedDataAsync();
        var meetingId = "meeting1";

        var result = await meetingService.GetDetailsAsync(meetingId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Test Meeting 1"));
        Assert.That(result.BookClubId, Is.EqualTo("bookclub1"));
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewMeeting()
    {
        var title = "Test Meeting";
        var description = "Test Description";
        var scheduledDate = DateTime.UtcNow.AddDays(1);
        var bookClubId = "club1";
        var bookId = "book1";

        var meetingId = await meetingService.CreateAsync(title, description, scheduledDate, bookClubId, bookId);

        var createdMeeting = await dbContext.Meetings.FindAsync(meetingId);
        Assert.That(createdMeeting, Is.Not.Null);
        Assert.That(createdMeeting!.Title, Is.EqualTo(title));
        Assert.That(createdMeeting.Description, Is.EqualTo(description));
    }

    [Test]
    public async Task DeleteAsync_ShouldSoftDeleteMeeting()
    {
        await SeedDataAsync();
        var meetingId = "meeting1";

        await meetingService.DeleteAsync(meetingId);

        var deletedMeeting = await dbContext.Meetings.FindAsync(meetingId);
        Assert.That(deletedMeeting!.IsDeleted, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        await SeedDataAsync();
        var meetingId = "meeting1";

        var result = await meetingService.ExistsAsync(meetingId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        await SeedDataAsync();
        var meetingId = "nonexistent";

        var result = await meetingService.ExistsAsync(meetingId);

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