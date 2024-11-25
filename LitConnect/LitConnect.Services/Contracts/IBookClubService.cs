namespace LitConnect.Services.Contracts;

using LitConnect.Web.ViewModels.BookClub;

public interface IBookClubService
{
    Task<IEnumerable<BookClubAllViewModel>> GetAllAsync(string userId);

    Task<BookClubDetailsViewModel?> GetDetailsAsync(string bookClubId, string userId);

    Task<string> CreateAsync(BookClubCreateViewModel model, string ownerId);

    Task JoinBookClubAsync(string bookClubId, string userId);

    Task LeaveBookClubAsync(string bookClubId, string userId);

    Task AddBookAsync(string bookClubId, string bookId, bool isCurrentlyReading);

    Task RemoveBookAsync(string bookClubId, string bookId);

    Task SetCurrentlyReadingAsync(string bookClubId, string bookId);

    Task<IEnumerable<BookClubBookViewModel>> GetBooksAsync(string bookClubId);
}