namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.BookClub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class BookClubController : Controller
{
    private readonly IBookClubService _bookClubService;
    private readonly IBookService _bookService;
    private readonly IBookClubMapper _bookClubMapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookClubController(
        IBookClubService bookClubService,
        IBookService bookService,
        IBookClubMapper bookClubMapper,
        UserManager<ApplicationUser> userManager)
    {
        _bookClubService = bookClubService;
        _bookService = bookService;
        _bookClubMapper = bookClubMapper;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var bookClubs = await _bookClubService.GetAllAsync(userId);
        var viewModels = _bookClubMapper.MapToAllViewModels(bookClubs);
        return View(viewModels);
    }


    public async Task<IActionResult> MyClubs()
    {
        var userId = _userManager.GetUserId(User);
        var bookClubs = await _bookClubService.GetUserClubsAsync(userId);
        var viewModels = _bookClubMapper.MapToAllViewModels(bookClubs);
        return View(viewModels);
    }

    public async Task<IActionResult> Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var bookClub = await _bookClubService.GetDetailsAsync(id, userId);

        if (bookClub == null)
        {
            return NotFound();
        }

        var viewModel = _bookClubMapper.MapToDetailsViewModel(bookClub);
        return View(viewModel);
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
        var bookClubId = await _bookClubService.CreateAsync(model.Name, model.Description, userId);

        return RedirectToAction(nameof(Details), new { id = bookClubId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(string id)
    {
        var userId = _userManager.GetUserId(User);
        var isAlreadyMember = await _bookClubService.IsUserMemberAsync(id, userId);

        if (!isAlreadyMember)
        {
            await _bookClubService.JoinBookClubAsync(id, userId);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(string id)
    {
        var userId = _userManager.GetUserId(User);
        await _bookClubService.LeaveBookClubAsync(id, userId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Members(string id)
    {
        var userId = _userManager.GetUserId(User);
        var bookClub = await _bookClubService.GetDetailsAsync(id, userId);

        if (bookClub == null)
        {
            return NotFound();
        }

        var isCurrentUserOwner = bookClub.OwnerId == userId;
        var isCurrentUserAdmin = bookClub.IsUserAdmin;

        var viewModel = _bookClubMapper.MapToMembersViewModel(bookClub, isCurrentUserOwner, isCurrentUserAdmin);
        return View(viewModel);
    }

    public async Task<IActionResult> AddBook(string id)
    {
        var books = await _bookService.GetAllAsync();
        ViewBag.Books = books;

        var model = new AddBookViewModel
        {
            BookClubId = id,
            BookId = string.Empty
        };

        return View(model);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAdmin(string bookClubId, string userId, bool isAdmin)
    {
        var currentUserId = _userManager.GetUserId(User);
        await _bookClubService.SetAdminStatusAsync(bookClubId, userId, currentUserId, isAdmin);
        return RedirectToAction(nameof(Members), new { id = bookClubId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = _userManager.GetUserId(User);
        await _bookClubService.DeleteAsync(id, userId);
        return RedirectToAction(nameof(Index));
    }
}