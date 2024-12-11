namespace LitConnect.Web.Controllers;

using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class MeetingController : Controller
{
    private readonly IMeetingService _meetingService;
    private readonly IBookService _bookService;
    private readonly IMeetingMapper _meetingMapper;

    public MeetingController(
        IMeetingService meetingService,
        IBookService bookService,
        IMeetingMapper meetingMapper)
    {
        _meetingService = meetingService;
        _bookService = bookService;
        _meetingMapper = meetingMapper;
    }

    public async Task<IActionResult> Create(string bookClubId)
    {
        var books = await _bookService.GetAllAsync();
        ViewBag.Books = books;

        var model = new MeetingCreateViewModel
        {
            BookClubId = bookClubId,
            ScheduledDate = DateTime.UtcNow.AddDays(1)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MeetingCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var books = await _bookService.GetAllAsync();
            ViewBag.Books = books;
            return View(model);
        }

        string meetingId = await _meetingService.CreateAsync(
            model.Title,
            model.Description,
            model.ScheduledDate,
            model.BookClubId,
            model.BookId);

        return RedirectToAction(nameof(Details), new { id = meetingId });
    }

    public async Task<IActionResult> Details(string id)
    {
        var meeting = await _meetingService.GetDetailsAsync(id);

        if (meeting == null)
        {
            return NotFound();
        }

        var viewModel = _meetingMapper.MapToDetailsViewModel(meeting);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, string bookClubId)
    {
        await _meetingService.DeleteAsync(id);
        return RedirectToAction("Details", "BookClub", new { id = bookClubId });
    }
}