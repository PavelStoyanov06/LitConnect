using LitConnect.Services.Contracts;
using LitConnect.Web.Controllers;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Book;
using LitConnect.Web.ViewModels.Genre;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using LitConnect.Services.Models;

namespace LitConnect.Web.Tests;

[TestFixture]
public class BookControllerTests : IDisposable
{
    private Mock<IBookService> bookServiceMock = null!;
    private Mock<IBookMapper> bookMapperMock = null!;
    private BookController controller = null!;
    private bool isDisposed;

    [SetUp]
    public void Setup()
    {
        bookServiceMock = new Mock<IBookService>();
        bookMapperMock = new Mock<IBookMapper>();
        controller = new BookController(bookServiceMock.Object, bookMapperMock.Object);
    }

    [Test]
    public async Task Index_ShouldReturnViewWithBooks()
    {
        var bookDtos = new List<BookDto>
        {
            new() { Id = "1", Title = "Book 1", Author = "Author 1" },
            new() { Id = "2", Title = "Book 2", Author = "Author 2" }
        };

        var expectedViewModels = new List<BookAllViewModel>
        {
            new() { Id = "1", Title = "Book 1", Author = "Author 1" },
            new() { Id = "2", Title = "Book 2", Author = "Author 2" }
        };

        bookServiceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(bookDtos);

        bookMapperMock.Setup(m => m.MapToAllViewModels(bookDtos))
            .Returns(expectedViewModels);

        var actionResult = await controller.Index();
        var result = actionResult as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedViewModels));
        });
    }

    [Test]
    public async Task Details_WithValidId_ShouldReturnViewWithBook()
    {
        var bookId = "1";
        var bookDto = new BookDto
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author"
        };

        var expectedViewModel = new BookDetailsViewModel
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author"
        };

        bookServiceMock.Setup(s => s.GetByIdAsync(bookId))
            .ReturnsAsync(bookDto);

        bookMapperMock.Setup(m => m.MapToDetailsViewModel(bookDto))
            .Returns(expectedViewModel);

        var actionResult = await controller.Details(bookId);
        var result = actionResult as ViewResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.EqualTo(expectedViewModel));
        });
    }

    [Test]
    public async Task Create_Post_WithValidModel_ShouldRedirectToDetails()
    {
        var model = new BookCreateViewModel
        {
            Title = "New Book",
            Author = "New Author",
            Description = "Description",
            GenreIds = new List<string> { "1", "2" }
        };
        var expectedBookId = "new_book_id";

        bookServiceMock.Setup(s => s.CreateAsync(
                model.Title,
                model.Author,
                model.Description,
                model.GenreIds))
            .ReturnsAsync(expectedBookId);

        var actionResult = await controller.Create(model);
        var result = actionResult as RedirectToActionResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("Details"));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(expectedBookId));
        });
    }

    [Test]
    public async Task Create_Get_ShouldReturnView()
    {
        var genreDtos = new List<GenreDto>
        {
            new() { Id = "1", Name = "Fiction" },
            new() { Id = "2", Name = "Non-Fiction" }
        };

        var genreViewModels = new List<GenreViewModel>
        {
            new() { Id = "1", Name = "Fiction" },
            new() { Id = "2", Name = "Non-Fiction" }
        };

        bookServiceMock.Setup(s => s.GetAllGenresAsync())
            .ReturnsAsync(genreDtos);

        var actionResult = await controller.Create();
        var result = actionResult as ViewResult;

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