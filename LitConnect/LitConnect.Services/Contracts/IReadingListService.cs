namespace LitConnect.Services.Contracts;

using LitConnect.Services.Models;

public interface IReadingListService
{
    Task<ReadingListDto> GetByUserIdAsync(string userId);

    Task AddBookAsync(string userId, string bookId);

    Task RemoveBookAsync(string userId, string bookId);

    Task UpdateBookStatusAsync(string userId, string bookId, ReadingStatus status);

    Task<bool> HasBookAsync(string userId, string bookId);
}