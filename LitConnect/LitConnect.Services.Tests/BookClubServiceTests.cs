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
        // Arrange
        var allBookClubs = await dbContext.BookClubs.ToListAsync();
        foreach (var bookClub in allBookClubs)
        {
            dbContext.BookClubs.Remove(bookClub);
        }
        await dbContext.SaveChangesAsync();

        await SeedDataAsync();

        // Act
        var result = await bookClubService.GetAllAsync("user1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectDetails()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user1";

        // Act
        var result = await bookClubService.GetDetailsAsync(bookClubId, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Test Book Club 1"));
        Assert.That(result.IsUserMember, Is.True);
        Assert.That(result.IsUserOwner, Is.True);
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewBookClub()
    {
        // Arrange
        var model = new BookClubCreateViewModel
        {
            Name = "New Book Club",
            Description = "New Description"
        };
        var ownerId = "user1";

        // Act
        var bookClubId = await bookClubService.CreateAsync(model, ownerId);

        // Assert
        var createdClub = await dbContext.BookClubs.FindAsync(bookClubId);
        Assert.That(createdClub, Is.Not.Null);
        Assert.That(createdClub!.Name, Is.EqualTo(model.Name));
        Assert.That(createdClub.OwnerId, Is.EqualTo(ownerId));
    }

    [Test]
    public async Task JoinBookClubAsync_ShouldAddUserToClub()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub2";
        var userId = "user2";

        // Act
        await bookClubService.JoinBookClubAsync(bookClubId, userId);

        // Assert
        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);
        Assert.That(membership, Is.Not.Null);
    }

    [Test]
    public async Task LeaveBookClubAsync_ShouldRemoveUserFromClub()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user2";

        // Act
        await bookClubService.LeaveBookClubAsync(bookClubId, userId);

        // Assert
        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);
        Assert.That(membership, Is.Null);
    }

    [Test]
    public async Task AddBookAsync_ShouldAddBookToClub()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var bookId = "book1";

        // Act
        await bookClubService.AddBookAsync(bookClubId, bookId, false);

        // Assert
        var bookClub = await dbContext.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId);
        Assert.That(bookClub!.Books.Any(b => b.Id == bookId), Is.True);
    }

    [Test]
    public async Task SetCurrentlyReadingAsync_ShouldUpdateCurrentBook()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var bookId = "book1";
        await bookClubService.AddBookAsync(bookClubId, bookId, false);

        // Act
        await bookClubService.SetCurrentlyReadingAsync(bookClubId, bookId);

        // Assert
        var bookClub = await dbContext.BookClubs.FindAsync(bookClubId);
        Assert.That(bookClub!.CurrentBookId, Is.EqualTo(bookId));
    }

    [Test]
    public async Task RemoveBookAsync_ShouldRemoveBookFromClub()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var bookId = "book1";
        await bookClubService.AddBookAsync(bookClubId, bookId, false);

        // Act
        await bookClubService.RemoveBookAsync(bookClubId, bookId);

        // Assert
        var bookClub = await dbContext.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId);
        Assert.That(bookClub!.Books.Any(b => b.Id == bookId), Is.False);
    }

    [Test]
    public async Task IsUserAdminAsync_WithAdmin_ShouldReturnTrue()
    {
        // Arrange
        await SeedDataAsync();
        var bookClubId = "bookclub1";
        var userId = "user2";
        await SetUserAsAdmin(bookClubId, userId);

        // Act
        var result = await bookClubService.IsUserAdminAsync(bookClubId, userId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task GetUserClubsAsync_ShouldReturnUserClubs()
    {
        // Arrange
        await SeedDataAsync();
        var userId = "user1";

        // Act
        var result = await bookClubService.GetUserClubsAsync(userId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    private async Task SeedDataAsync()
    {
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

        var bookClub1 = new BookClub
        {
            Id = "bookclub1",
            Name = "Test Book Club 1",
            Description = "Description 1",
            OwnerId = "user1",
            IsDeleted = false
        };

        var bookClub2 = new BookClub
        {
            Id = "bookclub2",
            Name = "Test Book Club 2",
            Description = "Description 2",
            OwnerId = "user1",
            IsDeleted = false
        };

        var membership = new UserBookClub
        {
            UserId = "user1",
            BookClubId = "bookclub1",
            JoinedOn = DateTime.UtcNow
        };

        var book = new Book
        {
            Id = "book1",
            Title = "Test Book",
            Author = "Test Author",
            IsDeleted = false
        };

        await dbContext.Users.AddRangeAsync(user1, user2);
        await dbContext.Books.AddAsync(book);
        await dbContext.BookClubs.AddRangeAsync(bookClub1, bookClub2);
        await dbContext.UsersBookClubs.AddAsync(membership);
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