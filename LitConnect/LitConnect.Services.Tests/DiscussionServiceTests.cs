using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class DiscussionServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private DiscussionService discussionService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectDiscussionTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new LitConnectDbContext(this.dbOptions);
        this.discussionService = new DiscussionService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        this.dbContext.Database.EnsureDeleted();
        this.dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }

    [Test]
    public async Task GetBookClubDiscussionsAsync_ShouldReturnAllNonDeletedDiscussions()
    {
        await SeedDataAsync();
        var bookClubId = "bookclub1";

        var result = await discussionService.GetBookClubDiscussionsAsync(bookClubId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2), "Should return only non-deleted discussions");
        });
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectDiscussion()
    {
        await SeedDataAsync();
        var userId = "user1";

        var result = await discussionService.GetDetailsAsync("discussion1", userId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("Test Discussion 1"));
            Assert.That(result.AuthorName, Is.EqualTo("Test User"));
            Assert.That(result.IsCurrentUserAuthor, Is.True);
        });
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewDiscussion()
    {
        var title = "Test Discussion";
        var content = "Test Content";
        var bookClubId = "club1";
        var bookId = "book1";
        var authorId = "user1";

        var discussionId = await discussionService.CreateAsync(title, content, bookClubId, bookId, authorId);

        var createdDiscussion = await dbContext.Discussions.FindAsync(discussionId);
        Assert.That(createdDiscussion, Is.Not.Null);
        Assert.That(createdDiscussion!.Title, Is.EqualTo(title));
        Assert.That(createdDiscussion.BookClubId, Is.EqualTo(bookClubId));
    }

    [Test]
    public async Task DeleteAsync_ShouldSoftDeleteDiscussion()
    {
        await SeedDataAsync();
        var discussionId = "discussion1";

        await discussionService.DeleteAsync(discussionId);
        var deletedDiscussion = await dbContext.Discussions.FindAsync(discussionId);

        Assert.Multiple(() =>
        {
            Assert.That(deletedDiscussion, Is.Not.Null);
            Assert.That(deletedDiscussion!.IsDeleted, Is.True);
        });
    }

    [Test]
    public async Task CanUserDeleteAsync_WithAuthor_ShouldReturnTrue()
    {
        await SeedDataAsync();
        var discussionId = "discussion1";
        var authorId = "user1";

        var result = await discussionService.CanUserDeleteAsync(discussionId, authorId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CanUserDeleteAsync_WithAdmin_ShouldReturnTrue()
    {
        await SeedDataAsync();
        var discussionId = "discussion1";
        var adminId = "admin1";

        var result = await discussionService.CanUserDeleteAsync(discussionId, adminId);

        Assert.That(result, Is.True);
    }

    private async Task SeedDataAsync()
    {
        var users = new[]
        {
            new ApplicationUser
            {
                Id = "user1",
                UserName = "user1@test.com",
                FirstName = "Test",
                LastName = "User",
                Email = "user1@test.com"
            },
            new ApplicationUser
            {
                Id = "admin1",
                UserName = "admin@test.com",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@test.com"
            }
        };

        var bookClub = new BookClub
        {
            Id = "bookclub1",
            Name = "Test Book Club",
            OwnerId = "user1"
        };

        var memberships = new[]
        {
            new UserBookClub
            {
                UserId = "user1",
                BookClubId = "bookclub1",
                JoinedOn = DateTime.UtcNow,
                IsAdmin = false
            },
            new UserBookClub
            {
                UserId = "admin1",
                BookClubId = "bookclub1",
                JoinedOn = DateTime.UtcNow,
                IsAdmin = true
            }
        };

        var discussions = new[]
        {
            new Discussion
            {
                Id = "discussion1",
                Title = "Test Discussion 1",
                Content = "Test Content 1",
                AuthorId = "user1",
                BookClubId = "bookclub1",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            },
            new Discussion
            {
                Id = "discussion2",
                Title = "Test Discussion 2",
                Content = "Test Content 2",
                AuthorId = "admin1",
                BookClubId = "bookclub1",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            },
            new Discussion
            {
                Id = "discussion3",
                Title = "Deleted Discussion",
                Content = "Deleted Content",
                AuthorId = "user1",
                BookClubId = "bookclub1",
                CreatedOn = DateTime.UtcNow,
                IsDeleted = true
            }
        };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.BookClubs.AddAsync(bookClub);
        await dbContext.UsersBookClubs.AddRangeAsync(memberships);
        await dbContext.Discussions.AddRangeAsync(discussions);
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

    ~DiscussionServiceTests()
    {
        Dispose(false);
    }
}