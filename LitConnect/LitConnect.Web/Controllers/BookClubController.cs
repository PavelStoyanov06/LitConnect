namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class BookClubController : Controller
{
    private readonly IBookClubService _bookClubService;
    private readonly IBookService _bookService;
    private readonly IDiscussionService _discussionService;
    private readonly IBookClubMapper _bookClubMapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookClubController(
        IBookClubService bookClubService,
        IBookService bookService,
        IDiscussionService discussionService,
        IBookClubMapper bookClubMapper,
        UserManager<ApplicationUser> userManager)
    {
        _bookClubService = bookClubService;
        _bookService = bookService;
        _discussionService = discussionService;
        _bookClubMapper = bookClubMapper;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var bookClubs = await _bookClubService.GetAllAsync();
        var viewModels = _bookClubMapper.MapToAllViewModels(bookClubs);
        return View(viewModels);
    }

    public async Task<IActionResult> Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var bookClubDto = await _bookClubService.GetDetailsAsync(id, userId);

        if (bookClubDto == null)
        {
            return NotFound();
        }

        var viewModel = _bookClubMapper.MapToDetailsViewModel(bookClubDto);
        return View(viewModel);
    }

    [Authorize]
    public async Task<IActionResult> MyClubs()
    {
        var userId = _userManager.GetUserId(User);
        var bookClubs = await _bookClubService.GetUserClubsAsync(userId);
        var viewModels = _bookClubMapper.MapToAllViewModels(bookClubs);
        return View(viewModels);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Join(string id)
    {
        var userId = _userManager.GetUserId(User);
        var isAlreadyMember = await _bookClubService.IsUserMemberAsync(id, userId);

        if (isAlreadyMember)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        await _bookClubService.JoinBookClubAsync(id, userId);
        return RedirectToAction(nameof(Details), new { id });
    }
}