namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.ReadingList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ReadingListController : Controller
{
    private readonly IReadingListService _readingListService;
    private readonly IReadingListMapper _readingListMapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReadingListController(
        IReadingListService readingListService,
        IReadingListMapper readingListMapper,
        UserManager<ApplicationUser> userManager)
    {
        _readingListService = readingListService;
        _readingListMapper = readingListMapper;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var readingList = await _readingListService.GetByUserIdAsync(userId);
        var viewModel = _readingListMapper.MapToViewModel(readingList);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook(string bookId)
    {
        var userId = _userManager.GetUserId(User);

        if (!await _readingListService.HasBookAsync(userId, bookId))
        {
            await _readingListService.AddBookAsync(userId, bookId);
        }

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
        await _readingListService.UpdateBookStatusAsync(userId, bookId, (Services.Models.ReadingStatus)status);
        return RedirectToAction(nameof(Index));
    }
}