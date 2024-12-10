namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Book;

public class BookMapper : IBookMapper
{
    public BookAllViewModel MapToAllViewModel(BookDto dto)
    {
        return new BookAllViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            Genres = dto.Genres.Select(g => g.Name).ToHashSet()
        };
    }

    public BookDetailsViewModel MapToDetailsViewModel(BookDto dto)
    {
        return new BookDetailsViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            Description = dto.Description,
            Genres = dto.Genres.Select(g => g.Name).ToHashSet(),
            BookClubsCount = dto.BookClubsCount
        };
    }

    public IEnumerable<BookAllViewModel> MapToAllViewModels(IEnumerable<BookDto> dtos)
    {
        return dtos.Select(MapToAllViewModel);
    }
}