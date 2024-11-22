namespace LitConnect.Services.Contracts;

using LitConnect.Web.ViewModels.Book;
using LitConnect.Web.ViewModels.Genre;

public interface IBookService
{
    Task<IEnumerable<BookAllViewModel>> GetAllAsync();

    Task<BookDetailsViewModel?> GetDetailsAsync(string id);

    Task<string> CreateAsync(BookCreateViewModel model);

    Task<IEnumerable<GenreViewModel>> GetAllGenresAsync();

    Task AddToBookClubAsync(string bookId, string bookClubId);

    Task RemoveFromBookClubAsync(string bookId, string bookClubId);

    Task<bool> ExistsAsync(string id);
}