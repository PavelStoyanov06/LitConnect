using LitConnect.Common;
using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class ReadingListServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private ReadingListService readingListService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectReadingListTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new LitConnectDbContext(this.dbOptions);
        this.readingListService = new ReadingListService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }

    [Test]
    public async Task GetByUserIdAsync_ShouldReturnUserReadingList()
    {
        await SeedDataAsync();
        var userId = "user1";

        var result = await readingListService.GetByUserIdAsync(userId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Books.Count(), Is.EqualTo(2));
            Assert.That(result.UserId, Is.EqualTo(userId));
        });
    }

    [Test]
    public async Task AddBookAsync_ShouldAddBookToReadingList()
    {
        await SeedDataAsync();
        var userId = "user1";
        var bookId = "book3";

        await readingListService.AddBookAsync(userId, bookId);

        var readingList = await dbContext.ReadingLists
            .Include(rl => rl.BookStatuses)
            .FirstOrDefaultAsync(rl => rl.UserId == userId);

        Assert.That(readingList!.BookStatuses.Any(bs => bs.BookId == bookId), Is.True);
        Assert.That(readingList.BookStatuses.First(bs => bs.BookId == bookId).Status,
            Is.EqualTo(ReadingStatus.WantToRead));
    }

    [Test]
    public async Task RemoveBookAsync_ShouldRemoveBookFromReadingList()
    {
        await SeedDataAsync();
        var userId = "user1";
        var bookId = "book1";

        await readingListService.RemoveBookAsync(userId, bookId);

        var readingList = await dbContext.ReadingLists
            .Include(rl => rl.BookStatuses)
            .FirstOrDefaultAsync(rl => rl.UserId == userId);

        Assert.That(readingList!.BookStatuses.Any(bs => bs.BookId == bookId), Is.False);
    }

    [Test]
    public async Task HasBookAsync_WithExistingBook_ShouldReturnTrue()
    {
        await SeedDataAsync();
        var userId = "user1";
        var bookId = "book1";

        var result = await readingListService.HasBookAsync(userId, bookId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasBookAsync_WithNonExistingBook_ShouldReturnFalse()
    {
        await SeedDataAsync();
        var userId = "user1";
        var bookId = "nonexistent";

        var result = await readingListService.HasBookAsync(userId, bookId);

        Assert.That(result, Is.False);
    }

    private async Task SeedDataAsync()
    {
        var user = new ApplicationUser
        {
            Id = "user1",
            UserName = "user1@test.com",
            Email = "user1@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        var books = new[]
        {
            new Book
            {
                Id = "book1",
                Title = "Test Book 1",
                Author = "Author 1",
                IsDeleted = false
            },
            new Book
            {
                Id = "book2",
                Title = "Test Book 2",
                Author = "Author 2",
                IsDeleted = false
            },
            new Book
            {
                Id = "book3",
                Title = "Test Book 3",
                Author = "Author 3",
                IsDeleted = false
            }
        };

        var readingList = new ReadingList
        {
            Id = "readinglist1",
            UserId = "user1",
            BookStatuses = new List<BookReadingStatus>
            {
                new BookReadingStatus
                {
                    BookId = "book1",
                    Status = ReadingStatus.WantToRead
                },
                new BookReadingStatus
                {
                    BookId = "book2",
                    Status = ReadingStatus.Reading
                }
            }
        };

        await dbContext.Users.AddAsync(user);
        await dbContext.Books.AddRangeAsync(books);
        await dbContext.ReadingLists.AddAsync(readingList);
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