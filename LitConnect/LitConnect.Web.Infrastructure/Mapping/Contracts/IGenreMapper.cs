namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.Genre;

public interface IGenreMapper
{
    GenreViewModel MapToViewModel(GenreDto dto);

    IEnumerable<GenreViewModel> MapToViewModels(IEnumerable<GenreDto> dtos);
}