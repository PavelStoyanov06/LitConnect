namespace LitConnect.Services.Contracts;

using LitConnect.Services.Models;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync();

    Task<BookDto?> GetByIdAsync(string id);

    Task<string> CreateAsync(string title, string author, string? description, IEnumerable<string> genreIds);

    Task DeleteAsync(string id);

    Task<IEnumerable<GenreDto>> GetAllGenresAsync();

    Task<bool> ExistsAsync(string id);
}