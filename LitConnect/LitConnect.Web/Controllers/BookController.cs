namespace LitConnect.Web.Controllers;

using LitConnect.Services.Contracts;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using Microsoft.AspNetCore.Mvc;

public class BookController : Controller
{
    private readonly IBookService _bookService;
    private readonly IBookMapper _bookMapper;

    public BookController(
        IBookService bookService,
        IBookMapper bookMapper)
    {
        _bookService = bookService;
        _bookMapper = bookMapper;
    }

    public async Task<IActionResult> Index()
    {
        var bookDtos = await _bookService.GetAllAsync();
        var viewModels = _bookMapper.MapToAllViewModels(bookDtos);
        return View(viewModels);
    }
}