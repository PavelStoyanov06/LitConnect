using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using LitConnect.Web.ViewModels.BookClub;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class BookClubServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private BookClubService bookClubService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectBookClubTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new TestLitConnectDbContext(this.dbOptions);
        this.bookClubService = new BookClubService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedBookClubs()
    {
        var allBookClubs = await dbContext.BookClubs.ToListAsync();
        foreach (var bookClub in allBookClubs)
        {
            dbContext.BookClubs.Remove(bookClub);
        }
        await dbContext.SaveChangesAsync();

        await SeedDataAsync();

        var result = await bookClubService.GetAllAsync("user1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectDetails()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user1";

        var result = await bookClubService.GetDetailsAsync(bookClubId, userId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Book Club 1"));
            Assert.That(result.IsUserMember, Is.True);
            Assert.That(result.IsUserOwner, Is.True);
        });
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewBookClub()
    {
        var model = new BookClubCreateViewModel
        {
            Name = "New Book Club",
            Description = "New Description"
        };
        var ownerId = "user1";

        var bookClubId = await bookClubService.CreateAsync(model, ownerId);

        var createdClub = await dbContext.BookClubs.FindAsync(bookClubId);
        Assert.That(createdClub, Is.Not.Null);
        Assert.That(createdClub!.Name, Is.EqualTo(model.Name));
        Assert.That(createdClub.OwnerId, Is.EqualTo(ownerId));
    }

    [Test]
    public async Task JoinBookClubAsync_ShouldAddUserToClub()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub2";
        var userId = "user2";

        await bookClubService.JoinBookClubAsync(bookClubId, userId);

        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);
        Assert.That(membership, Is.Not.Null);
    }

    [Test]
    public async Task JoinBookClubAsync_WithPreviouslyLeftClub_ShouldReactivateMembership()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user2";

        await bookClubService.JoinBookClubAsync(bookClubId, userId);
        await bookClubService.LeaveBookClubAsync(bookClubId, userId);

        await bookClubService.JoinBookClubAsync(bookClubId, userId);

        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        Assert.Multiple(() =>
        {
            Assert.That(membership, Is.Not.Null);
            Assert.That(membership!.IsDeleted, Is.False);
        });
    }

    [Test]
    public async Task LeaveBookClubAsync_ShouldSoftDeleteMembership()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user2";

        await bookClubService.LeaveBookClubAsync(bookClubId, userId);

        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        Assert.Multiple(() =>
        {
            Assert.That(membership, Is.Not.Null);
            Assert.That(membership!.IsDeleted, Is.True);
        });
    }

    [Test]
    public async Task AddBookAsync_ShouldAddBookToClub()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var bookId = "book1";

        await bookClubService.AddBookAsync(bookClubId, bookId, false);

        var bookClub = await dbContext.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId);
        Assert.That(bookClub!.Books.Any(b => b.Id == bookId), Is.True);
    }

    [Test]
    public async Task SetCurrentlyReadingAsync_ShouldUpdateCurrentBook()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var bookId = "book1";

        await bookClubService.AddBookAsync(bookClubId, bookId, false);

        await bookClubService.SetCurrentlyReadingAsync(bookClubId, bookId);

        var bookClub = await dbContext.BookClubs.FindAsync(bookClubId);
        Assert.That(bookClub!.CurrentBookId, Is.EqualTo(bookId));
    }

    [Test]
    public async Task RemoveBookAsync_ShouldRemoveBookFromClub()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var bookId = "book1";

        await bookClubService.AddBookAsync(bookClubId, bookId, false);

        await bookClubService.RemoveBookAsync(bookClubId, bookId);

        var bookClub = await dbContext.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId);
        Assert.That(bookClub!.Books.Any(b => b.Id == bookId), Is.False);
    }

    [Test]
    public async Task IsUserAdminAsync_WithAdmin_ShouldReturnTrue()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user2";
        await SetUserAsAdmin(bookClubId, userId);

        var result = await bookClubService.IsUserAdminAsync(bookClubId, userId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task GetUserClubsAsync_ShouldReturnUserClubs()
    {
        await SeedDataAsync();
        var userId = "user1";

        var result = await bookClubService.GetUserClubsAsync(userId);

        Assert.That(result.Count(), Is.EqualTo(1));
    }

    private async Task SeedDataAsync()
    {
        // Clear existing data first
        var existingBookClubs = await dbContext.BookClubs.ToListAsync();
        var existingUsers = await dbContext.Users.ToListAsync();
        var existingMemberships = await dbContext.UsersBookClubs.ToListAsync();
        var existingBooks = await dbContext.Books.ToListAsync();
        var existingDiscussions = await dbContext.Discussions.ToListAsync();
        var existingMeetings = await dbContext.Meetings.ToListAsync();

        dbContext.Discussions.RemoveRange(existingDiscussions);
        dbContext.Meetings.RemoveRange(existingMeetings);
        dbContext.UsersBookClubs.RemoveRange(existingMemberships);
        dbContext.BookClubs.RemoveRange(existingBookClubs);
        dbContext.Books.RemoveRange(existingBooks);
        dbContext.Users.RemoveRange(existingUsers);
        await dbContext.SaveChangesAsync();

        var user1 = new ApplicationUser
        {
            Id = "user1",
            UserName = "user1@test.com",
            Email = "user1@test.com",
            FirstName = "Test",
            LastName = "User 1"
        };

        var user2 = new ApplicationUser
        {
            Id = "user2",
            UserName = "user2@test.com",
            Email = "user2@test.com",
            FirstName = "Test",
            LastName = "User 2"
        };

        await dbContext.Users.AddRangeAsync(user1, user2);
        await dbContext.SaveChangesAsync();

        var book = new Book
        {
            Id = "book1",
            Title = "Test Book",
            Author = "Test Author",
            IsDeleted = false
        };

        await dbContext.Books.AddAsync(book);
        await dbContext.SaveChangesAsync();

        var bookClub1 = new BookClub
        {
            Id = "bookclub1",
            Name = "Test Book Club 1",
            Description = "Description 1",
            OwnerId = user1.Id,
            IsDeleted = false,
            Books = new List<Book> { book }
        };

        var bookClub2 = new BookClub
        {
            Id = "bookclub2",
            Name = "Test Book Club 2",
            Description = "Description 2",
            OwnerId = user1.Id,
            IsDeleted = false
        };

        await dbContext.BookClubs.AddRangeAsync(bookClub1, bookClub2);
        await dbContext.SaveChangesAsync();

        var membership = new UserBookClub
        {
            UserId = user1.Id,
            BookClubId = bookClub1.Id,
            JoinedOn = DateTime.UtcNow,
            IsAdmin = false
        };

        await dbContext.UsersBookClubs.AddAsync(membership);
        await dbContext.SaveChangesAsync();

        // Add a meeting and discussion for complete relationship testing
        var meeting = new Meeting
        {
            Id = "meeting1",
            Title = "Test Meeting",
            BookClubId = bookClub1.Id,
            ScheduledDate = DateTime.UtcNow.AddDays(1)
        };

        var discussion = new Discussion
        {
            Id = "discussion1",
            Title = "Test Discussion",
            Content = "Test Content",
            BookClubId = bookClub1.Id,
            AuthorId = user1.Id,
            CreatedOn = DateTime.UtcNow
        };

        await dbContext.Meetings.AddAsync(meeting);
        await dbContext.Discussions.AddAsync(discussion);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
    }

    private async Task SetUserAsAdmin(string bookClubId, string userId)
    {
        var membership = new UserBookClub
        {
            UserId = userId,
            BookClubId = bookClubId,
            JoinedOn = DateTime.UtcNow,
            IsAdmin = true
        };
        await dbContext.UsersBookClubs.AddAsync(membership);
        await dbContext.SaveChangesAsync();
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