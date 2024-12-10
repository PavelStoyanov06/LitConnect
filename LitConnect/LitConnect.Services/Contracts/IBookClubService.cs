namespace LitConnect.Services.Contracts;

using LitConnect.Services.Models;

public interface IBookClubService
{
    Task<IEnumerable<BookClubDto>> GetAllAsync();

    Task<BookClubDto?> GetByIdAsync(string id);

    Task<string> CreateAsync(string name, string description, string ownerId);

    Task JoinBookClubAsync(string bookClubId, string userId);

    Task LeaveBookClubAsync(string bookClubId, string userId);

    Task AddBookAsync(string bookClubId, string bookId, bool isCurrentlyReading);

    Task RemoveBookAsync(string bookClubId, string bookId);

    Task SetCurrentlyReadingAsync(string bookClubId, string bookId);

    Task<IEnumerable<BookDto>> GetBooksAsync(string bookClubId);

    Task<BookDto?> GetCurrentBookAsync(string bookClubId);

    Task<IEnumerable<MemberDto>> GetMembersAsync(string bookClubId);

    Task SetAdminStatusAsync(string bookClubId, string userId, string currentUserId, bool isAdmin);

    Task<bool> IsUserAdminAsync(string bookClubId, string userId);

    Task<bool> IsUserOwnerOrAdminAsync(string bookClubId, string userId);

    Task<IEnumerable<BookClubDto>> GetUserClubsAsync(string userId);

    Task<BookClubDto> GetDetailsAsync(string id, string userId);
}