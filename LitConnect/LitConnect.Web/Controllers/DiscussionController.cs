namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Discussion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DiscussionController : Controller
{
    private readonly IDiscussionService _discussionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DiscussionController(
        IDiscussionService discussionService,
        UserManager<ApplicationUser> userManager)
    {
        _discussionService = discussionService;
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
        var discussionId = await _discussionService.CreateAsync(model, userId);

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

        return View(discussion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = _userManager.GetUserId(User);
        if (!await _discussionService.IsUserAuthorAsync(id, userId))
        {
            return Forbid();
        }

        await _discussionService.DeleteAsync(id);
        return RedirectToAction("Details", "BookClub", new { id = id });
    }
}