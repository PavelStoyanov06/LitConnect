namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CommentController : Controller
{
    private readonly ICommentService _commentService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentController(
        ICommentService commentService,
        UserManager<ApplicationUser> userManager)
    {
        _commentService = commentService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommentCreateViewModel model)
    {
        // Add this to see what's being received
        Console.WriteLine($"DiscussionId: {model.DiscussionId}");

        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Discussion", new { id = model.DiscussionId });
        }

        var userId = _userManager.GetUserId(User);
        await _commentService.CreateAsync(model, userId);

        return RedirectToAction("Details", "Discussion", new { id = model.DiscussionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, string discussionId)
    {
        var userId = _userManager.GetUserId(User);
        if (await _commentService.IsUserAuthorAsync(id, userId))
        {
            await _commentService.DeleteAsync(id);
        }

        return RedirectToAction("Details", "Discussion", new { id = discussionId });
    }
}