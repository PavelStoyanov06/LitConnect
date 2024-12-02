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

    Task<BookClubBookViewModel?> GetCurrentBookAsync(string bookClubId);

    Task<BookClubMembersViewModel> GetMembersAsync(string bookClubId, string currentUserId);

    Task SetAdminStatusAsync(string bookClubId, string userId, string currentUserId, bool isAdmin);

    Task<bool> IsUserAdminAsync(string bookClubId, string userId);

    Task<bool> IsUserOwnerOrAdminAsync(string bookClubId, string userId);

    Task<IEnumerable<BookClubAllViewModel>> GetUserClubsAsync(string userId);
}