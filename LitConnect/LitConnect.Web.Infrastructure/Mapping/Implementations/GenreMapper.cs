namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Genre;

public class GenreMapper : IGenreMapper
{
    public GenreViewModel MapToViewModel(GenreDto dto)
    {
        return new GenreViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            BooksCount = dto.BooksCount
        };
    }

    public IEnumerable<GenreViewModel> MapToViewModels(IEnumerable<GenreDto> dtos)
    {
        return dtos.Select(MapToViewModel);
    }
}