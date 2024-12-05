using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using LitConnect.Web.ViewModels.Book;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class BookServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private BookService bookService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectBookTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new LitConnectDbContext(this.dbOptions);
        this.bookService = new BookService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        // Reset the database before each test
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedBooks()
    {
        // Arrange
        await SeedBooksAsync();

        // Act
        var result = await bookService.GetAllAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2), "Should return only non-deleted books");
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectBook()
    {
        // Arrange
        await SeedBooksAsync();
        var testBookId = "book1";

        // Act
        var result = await bookService.GetDetailsAsync(testBookId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Test Book 1"));
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewBook()
    {
        // Arrange
        await SeedBooksAsync();
        var model = new BookCreateViewModel
        {
            Title = "New Book",
            Author = "New Author",
            Description = "New Description",
            GenreIds = new[] { "genre1" }
        };

        // Act
        var bookId = await bookService.CreateAsync(model);

        // Assert
        var createdBook = await dbContext.Books.FindAsync(bookId);
        Assert.That(createdBook, Is.Not.Null);
        Assert.That(createdBook!.Title, Is.EqualTo(model.Title));
        Assert.That(createdBook.Author, Is.EqualTo(model.Author));
    }

    [Test]
    public async Task ExistsAsync_WithExistingBook_ShouldReturnTrue()
    {
        // Arrange
        await SeedBooksAsync();
        var bookId = "book1";

        // Act
        var result = await bookService.ExistsAsync(bookId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithNonExistingBook_ShouldReturnFalse()
    {
        // Arrange
        await SeedBooksAsync();
        var bookId = "nonexistent";

        // Act
        var result = await bookService.ExistsAsync(bookId);

        // Assert
        Assert.That(result, Is.False);
    }

    private async Task SeedBooksAsync()
    {
        // Add test genres
        await dbContext.Genres.AddRangeAsync(new[]
        {
            new Genre { Id = "genre1", Name = "Fiction", IsDeleted = false },
            new Genre { Id = "genre2", Name = "Non-Fiction", IsDeleted = false }
        });

        // Add test books
        await dbContext.Books.AddRangeAsync(new[]
        {
            new Book
            {
                Id = "book1",
                Title = "Test Book 1",
                Author = "Test Author 1",
                Description = "Test Description 1",
                IsDeleted = false
            },
            new Book
            {
                Id = "book2",
                Title = "Test Book 2",
                Author = "Test Author 2",
                Description = "Test Description 2",
                IsDeleted = false
            },
            new Book
            {
                Id = "book3",
                Title = "Deleted Book",
                Author = "Deleted Author",
                Description = "This book is deleted",
                IsDeleted = true
            }
        });

        // Add book-genre relationships
        await dbContext.BooksGenres.AddRangeAsync(new[]
        {
            new BookGenre { BookId = "book1", GenreId = "genre1", IsDeleted = false },
            new BookGenre { BookId = "book2", GenreId = "genre2", IsDeleted = false }
        });

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