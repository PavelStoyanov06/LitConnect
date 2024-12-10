namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.Book;

public interface IBookMapper
{
    BookAllViewModel MapToAllViewModel(BookDto dto);
    BookDetailsViewModel MapToDetailsViewModel(BookDto dto);
    IEnumerable<BookAllViewModel> MapToAllViewModels(IEnumerable<BookDto> dtos);
}
