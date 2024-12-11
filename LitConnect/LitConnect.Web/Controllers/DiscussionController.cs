namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Discussion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DiscussionController : Controller
{
    private readonly IDiscussionService _discussionService;
    private readonly IDiscussionMapper _discussionMapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public DiscussionController(
        IDiscussionService discussionService,
        IDiscussionMapper discussionMapper,
        UserManager<ApplicationUser> userManager)
    {
        _discussionService = discussionService;
        _discussionMapper = discussionMapper;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create(string bookClubId)
    {
        var model = new DiscussionCreateViewModel
        {
            BookClubId = bookClubId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DiscussionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        var discussionId = await _discussionService.CreateAsync(
            model.Title,
            model.Content,
            model.BookClubId,
            model.BookId,
            userId);

        return RedirectToAction(nameof(Details), new { id = discussionId });
    }

    public async Task<IActionResult> Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var discussion = await _discussionService.GetDetailsAsync(id, userId);

        if (discussion == null)
        {
            return NotFound();
        }

        var viewModel = _discussionMapper.MapToDetailsViewModel(discussion);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = _userManager.GetUserId(User);

        if (!await _discussionService.CanUserDeleteAsync(id, userId))
        {
            return Forbid();
        }

        var discussion = await _discussionService.GetDetailsAsync(id, userId);
        if (discussion == null)
        {
            return NotFound();
        }

        await _discussionService.DeleteAsync(id);
        return RedirectToAction("Details", "BookClub", new { id = discussion.BookClubId });
    }
}