namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.BookClub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class BookClubController : Controller
{
    private readonly IBookClubService _bookClubService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookClubController(
        IBookClubService bookClubService,
        UserManager<ApplicationUser> userManager)
    {
        _bookClubService = bookClubService;
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
}