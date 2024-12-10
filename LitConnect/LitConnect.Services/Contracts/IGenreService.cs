namespace LitConnect.Services.Contracts;

using LitConnect.Services.Models;

public interface IGenreService
{
    Task<IEnumerable<GenreDto>> GetAllAsync();

    Task<GenreDto?> GetByIdAsync(string id);

    Task<bool> ExistsAsync(string id);

    Task<int> GetBooksCountAsync(string id);
}