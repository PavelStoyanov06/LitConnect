using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class MeetingController : Controller
{
    private readonly IMeetingService _meetingService;
    private readonly IMeetingMapper _meetingMapper;
    private readonly IBookService _bookService;

    public MeetingController(
        IMeetingService meetingService,
        IMeetingMapper meetingMapper,
        IBookService bookService)
    {
        _meetingService = meetingService;
        _meetingMapper = meetingMapper;
        _bookService = bookService;
    }

    public async Task<IActionResult> Details(string id)
    {
        var meeting = await _meetingService.GetByIdAsync(id);

        if (meeting == null)
        {
            return NotFound();
        }

        var viewModel = _meetingMapper.MapToDetailsViewModel(meeting);
        return View(viewModel);
    }
}
