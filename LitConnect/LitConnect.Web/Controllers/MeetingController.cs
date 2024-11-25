namespace LitConnect.Web.Controllers;

using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class MeetingController : Controller
{
    private readonly IMeetingService _meetingService;
    private readonly IBookService _bookService;

    public MeetingController(
        IMeetingService meetingService,
        IBookService bookService)
    {
        _meetingService = meetingService;
        _bookService = bookService;
    }

    public async Task<IActionResult> Create(string bookClubId)
    {
        var model = new MeetingCreateViewModel
        {
            BookClubId = bookClubId,
            ScheduledDate = DateTime.Now.AddDays(1).Date.AddHours(18) // Default to tomorrow at 6 PM
        };

        ViewBag.Books = await _bookService.GetAllAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MeetingCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Books = await _bookService.GetAllAsync();
            return View(model);
        }

        var meetingId = await _meetingService.CreateAsync(model);
        return RedirectToAction(nameof(Details), new { id = meetingId });
    }

    public async Task<IActionResult> Details(string id)
    {
        var meeting = await _meetingService.GetDetailsAsync(id);

        if (meeting == null)
        {
            return NotFound();
        }

        return View(meeting);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, string bookClubId)
    {
        await _meetingService.DeleteAsync(id);
        return RedirectToAction("Details", "BookClub", new { id = bookClubId });
    }
}