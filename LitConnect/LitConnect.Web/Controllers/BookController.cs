namespace LitConnect.Web.Controllers;

using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class BookController : Controller
{
    private readonly IBookService _bookService;
    private readonly IBookMapper _bookMapper;

    public BookController(
        IBookService bookService,
        IBookMapper bookMapper)
    {
        _bookService = bookService;
        _bookMapper = bookMapper;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync();
        var viewModels = _bookMapper.MapToAllViewModels(books);
        return View(viewModels);
    }

    public async Task<IActionResult> Details(string id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        var viewModel = _bookMapper.MapToDetailsViewModel(book);
        return View(viewModel);
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Genres = await _bookService.GetAllGenresAsync();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Genres = await _bookService.GetAllGenresAsync();
            return View(model);
        }

        string bookId = await _bookService.CreateAsync(
            model.Title,
            model.Author,
            model.Description,
            model.GenreIds);

        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        await _bookService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}