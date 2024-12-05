using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using LitConnect.Web.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class CommentServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private CommentService commentService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectCommentTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new LitConnectDbContext(this.dbOptions);
        this.commentService = new CommentService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        this.dbContext.Database.EnsureDeleted();
        this.dbContext.Database.EnsureCreated();
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewComment()
    {
        // Arrange
        await SeedDataAsync();
        var model = new CommentCreateViewModel
        {
            Content = "Test Comment",
            DiscussionId = "discussion1"
        };
        var authorId = "user1";

        // Act
        var commentId = await commentService.CreateAsync(model, authorId);
        var createdComment = await dbContext.Comments.FindAsync(commentId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(createdComment, Is.Not.Null);
            Assert.That(createdComment!.Content, Is.EqualTo(model.Content));
            Assert.That(createdComment.AuthorId, Is.EqualTo(authorId));
            Assert.That(createdComment.DiscussionId, Is.EqualTo(model.DiscussionId));
        });
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveComment()
    {
        // Arrange
        await SeedDataAsync();
        var commentId = "comment1";

        // Act
        await commentService.DeleteAsync(commentId);
        var deletedComment = await dbContext.Comments.FindAsync(commentId);

        // Assert
        Assert.That(deletedComment, Is.Null);
    }

    [Test]
    public async Task IsUserAuthorAsync_WithCorrectAuthor_ShouldReturnTrue()
    {
        // Arrange
        await SeedDataAsync();
        var commentId = "comment1";
        var authorId = "user1";

        // Act
        var result = await commentService.IsUserAuthorAsync(commentId, authorId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsUserAuthorAsync_WithWrongAuthor_ShouldReturnFalse()
    {
        // Arrange
        await SeedDataAsync();
        var commentId = "comment1";
        var wrongAuthorId = "user2";

        // Act
        var result = await commentService.IsUserAuthorAsync(commentId, wrongAuthorId);

        // Assert
        Assert.That(result, Is.False);
    }

    private async Task SeedDataAsync()
    {
        var user = new ApplicationUser
        {
            Id = "user1",
            UserName = "testuser@test.com",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@test.com"
        };

        var discussion = new Discussion
        {
            Id = "discussion1",
            Title = "Test Discussion",
            Content = "Test Content",
            AuthorId = "user1",
            BookClubId = "bookclub1",
            CreatedOn = DateTime.UtcNow
        };

        var comment = new Comment
        {
            Id = "comment1",
            Content = "Existing Comment",
            AuthorId = "user1",
            DiscussionId = "discussion1",
            CreatedOn = DateTime.UtcNow
        };

        await dbContext.Users.AddAsync(user);
        await dbContext.Discussions.AddAsync(discussion);
        await dbContext.Comments.AddAsync(comment);
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

    ~CommentServiceTests()
    {
        Dispose(false);
    }
}