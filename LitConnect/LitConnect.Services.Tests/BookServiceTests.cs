using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectBookTestDb_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [SetUp]
    public void Setup()
    {
        dbContext = new LitConnectDbContext(dbOptions);
        bookService = new BookService(dbContext);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }


    [Test]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedBooks()
    {
        await SeedBooksAsync();

        var result = await bookService.GetAllAsync();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2), "Should return only non-deleted books");
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectBook()
    {
        await SeedBooksAsync();
        var testBookId = "book1";

        var result = await bookService.GetByIdAsync(testBookId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Test Book 1"));
    }

    [Test]
    public async Task CreateAsync_ShouldCreateNewBook()
    {
        var title = "New Book";
        var author = "New Author";
        var description = "New Description";
        var genreIds = new[] { "genre1" };

        var bookId = await bookService.CreateAsync(title, author, description, genreIds);

        var createdBook = await dbContext.Books.FindAsync(bookId);
        Assert.That(createdBook, Is.Not.Null);
        Assert.That(createdBook!.Title, Is.EqualTo(title));
        Assert.That(createdBook.Author, Is.EqualTo(author));
    }

    [Test]
    public async Task ExistsAsync_WithExistingBook_ShouldReturnTrue()
    {
        await SeedBooksAsync();
        var bookId = "book1";

        var result = await bookService.ExistsAsync(bookId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithNonExistingBook_ShouldReturnFalse()
    {
        await SeedBooksAsync();
        var bookId = "nonexistent";

        var result = await bookService.ExistsAsync(bookId);

        Assert.That(result, Is.False);
    }

    private async Task SeedBooksAsync()
    {
        var existingBooks = await dbContext.Books.ToListAsync();
        var existingGenres = await dbContext.Genres.ToListAsync();
        var existingBookGenres = await dbContext.BooksGenres.ToListAsync();

        dbContext.BooksGenres.RemoveRange(existingBookGenres);
        dbContext.Books.RemoveRange(existingBooks);
        dbContext.Genres.RemoveRange(existingGenres);
        await dbContext.SaveChangesAsync();

        await dbContext.Genres.AddRangeAsync(new[]
        {
            new Genre { Id = "genre1", Name = "Fiction", IsDeleted = false },
            new Genre { Id = "genre2", Name = "Non-Fiction", IsDeleted = false }
        });

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

    [Test]
    public async Task DeleteAsync_ShouldSoftDeleteBook()
    {
        await SeedBooksAsync();

        const string bookId = "book1";

        await bookService.DeleteAsync(bookId);

        var deletedBook = await dbContext.Books.FindAsync(bookId);
        Assert.That(deletedBook!.IsDeleted, Is.True);
    }
}