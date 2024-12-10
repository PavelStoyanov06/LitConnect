namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
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
        var readingListDto = await _readingListService.GetByUserIdAsync(userId);
        var viewModel = _readingListMapper.MapToViewModel(readingListDto);

        return View(viewModel);
    }
}