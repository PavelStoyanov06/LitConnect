namespace LitConnect.Services.Contracts;

using LitConnect.Web.ViewModels.Genre;

public interface IGenreService
{
    Task<IEnumerable<GenreViewModel>> GetAllAsync();
    Task<GenreViewModel?> GetByIdAsync(string id);
    Task<bool> ExistsAsync(string id);
}