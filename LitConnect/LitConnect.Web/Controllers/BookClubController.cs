namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Implementations;
using LitConnect.Web.ViewModels.BookClub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class BookClubController : Controller
{
    private readonly IBookClubService _bookClubService;
    private readonly IBookService _bookService;
    private readonly IDiscussionService _discussionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookClubController(
        IBookClubService bookClubService,
        IBookService bookService,
        IDiscussionService discussionService,
        UserManager<ApplicationUser> userManager)
    {
        _bookClubService = bookClubService;
        _bookService = bookService;
        _discussionService = discussionService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var bookClubs = await _bookClubService.GetAllAsync(userId);

        return View(bookClubs);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookClubCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        var bookClubId = await _bookClubService.CreateAsync(model, userId);

        return RedirectToAction(nameof(Details), new { id = bookClubId });
    }

    public async Task<IActionResult> Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var bookClub = await _bookClubService.GetDetailsAsync(id, userId);

        if (bookClub == null)
        {
            return NotFound();
        }

        bookClub.Discussions = await _discussionService.GetBookClubDiscussionsAsync(id);

        return View(bookClub);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(string id)
    {
        var userId = _userManager.GetUserId(User);
        await _bookClubService.JoinBookClubAsync(id, userId);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(string id)
    {
        var userId = _userManager.GetUserId(User);
        await _bookClubService.LeaveBookClubAsync(id, userId);

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> AddBook(string id)
    {
        var books = await _bookService.GetAllAsync();
        ViewBag.Books = books;

        return View(new AddBookViewModel
        {
            BookClubId = id,
            BookId = string.Empty // Initialize the required property
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook(AddBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var books = await _bookService.GetAllAsync();
            ViewBag.Books = books;
            return View(model);
        }

        await _bookClubService.AddBookAsync(model.BookClubId, model.BookId, model.IsCurrentlyReading);
        return RedirectToAction(nameof(Details), new { id = model.BookClubId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBook(string bookClubId, string bookId)
    {
        await _bookClubService.RemoveBookAsync(bookClubId, bookId);
        return RedirectToAction(nameof(Details), new { id = bookClubId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCurrentlyReading(string bookClubId, string bookId)
    {
        await _bookClubService.SetCurrentlyReadingAsync(bookClubId, bookId);
        return RedirectToAction(nameof(Details), new { id = bookClubId });
    }
}