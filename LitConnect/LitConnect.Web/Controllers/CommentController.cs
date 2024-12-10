namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CommentController : Controller
{
    private readonly ICommentService _commentService;
    private readonly ICommentMapper _commentMapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentController(
        ICommentService commentService,
        ICommentMapper commentMapper,
        UserManager<ApplicationUser> userManager)
    {
        _commentService = commentService;
        _commentMapper = commentMapper;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommentCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Discussion", new { id = model.DiscussionId });
        }

        var userId = _userManager.GetUserId(User);
        await _commentService.CreateAsync(model.Content, model.DiscussionId, userId);

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