using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.ViewModels.Book;
using LitConnect.Web.ViewModels.Genre;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace LitConnect.Web.Tests;

[TestFixture]
public class BookControllerTests : IDisposable
{
    private Mock<IBookService> bookServiceMock = null!;
    private BookController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        bookServiceMock = new Mock<IBookService>();
        controller = new BookController(bookServiceMock.Object);
    }

    [Test]
    public async Task Index_ShouldReturnViewWithBooks()
    {
        // Arrange
        var expectedBooks = new List<BookAllViewModel>
        {
            new()
            {
                Id = "1",
                Title = "Book 1",
                Author = "Author 1",
                Genres = new List<string>()
            },
            new()
            {
                Id = "2",
                Title = "Book 2",
                Author = "Author 2",
                Genres = new List<string>()
            }
        };

        bookServiceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(expectedBooks);

        // Act
        var actionResult = await controller.Index();
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedBooks));
        });
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithBook()
    {
        // Arrange
        var bookId = "1";
        var expectedBook = new BookDetailsViewModel
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Genres = new HashSet<string>()
        };

        bookServiceMock.Setup(s => s.GetDetailsAsync(bookId))
            .ReturnsAsync(expectedBook);

        // Act
        var actionResult = await controller.Details(bookId);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedBook));
        });
    }

    [Test]
    public async Task Details_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var bookId = "nonexistent";
        bookServiceMock.Setup(s => s.GetDetailsAsync(bookId))
            .ReturnsAsync((BookDetailsViewModel?)null);

        // Act
        var result = await controller.Details(bookId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_Post_WithValidModel_ShouldRedirectToDetails()
    {
        // Arrange
        var model = new BookCreateViewModel
        {
            Title = "New Book",
            Author = "New Author",
            Description = "Description",
            GenreIds = new List<string> { "1", "2" }
        };
        var expectedBookId = "new_book_id";

        bookServiceMock.Setup(s => s.CreateAsync(model))
            .ReturnsAsync(expectedBookId);

        // Act
        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(expectedBookId));
        });
    }

    [Test]
    public async Task Create_Post_WithInvalidModel_ShouldReturnView()
    {
        // Arrange
        var model = new BookCreateViewModel
        {
            Title = "New Book",
            Author = "New Author",
            Description = "Description",
            GenreIds = new List<string> { "1", "2" }
        };
        controller.ModelState.AddModelError("Title", "Required");

        // Act
        var actionResult = await controller.Create(model);
        var result = actionResult as ViewResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(model));
        });
    }

    [Test]
    public async Task Create_Get_ShouldReturnView()
    {
        // Arrange
        var genres = new List<GenreViewModel>
        {
            new() { Id = "1", Name = "Fiction", BooksCount = 0 },
            new() { Id = "2", Name = "Non-Fiction", BooksCount = 0 }
        };

        bookServiceMock.Setup(s => s.GetAllGenresAsync())
            .ReturnsAsync(genres);

        // Act
        var actionResult = await controller.Create();
        var result = actionResult as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
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