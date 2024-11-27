namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.ReadingList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ReadingListController : Controller
{
    private readonly IReadingListService _readingListService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReadingListController(
        IReadingListService readingListService,
        UserManager<ApplicationUser> userManager)
    {
        _readingListService = readingListService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var readingList = await _readingListService.GetByUserIdAsync(userId);
        return View(readingList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook(string bookId)
    {
        var userId = _userManager.GetUserId(User);
        await _readingListService.AddBookAsync(userId, bookId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBook(string bookId)
    {
        var userId = _userManager.GetUserId(User);
        await _readingListService.RemoveBookAsync(userId, bookId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string bookId, ReadingStatus status)
    {
        var userId = _userManager.GetUserId(User);
        await _readingListService.UpdateBookStatusAsync(userId, bookId, status);
        return RedirectToAction(nameof(Index));
    }
}