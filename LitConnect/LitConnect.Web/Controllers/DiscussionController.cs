namespace LitConnect.Web.Controllers;

using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
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

    public async Task<IActionResult> Details(string id)
    {
        var userId = _userManager.GetUserId(User);
        var discussionDto = await _discussionService.GetDetailsAsync(id, userId);

        if (discussionDto == null)
        {
            return NotFound();
        }

        var viewModel = _discussionMapper.MapToDetailsViewModel(discussionDto);
        return View(viewModel);
    }
}