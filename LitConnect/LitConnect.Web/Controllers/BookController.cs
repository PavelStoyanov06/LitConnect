namespace LitConnect.Web.Controllers;

using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class BookController : Controller
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllAsync();
        return View(books);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Genres = await _bookService.GetAllGenresAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Genres = await _bookService.GetAllGenresAsync();
            return View(model);
        }

        var bookId = await _bookService.CreateAsync(model);
        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    public async Task<IActionResult> Details(string id)
    {
        var book = await _bookService.GetDetailsAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }
}