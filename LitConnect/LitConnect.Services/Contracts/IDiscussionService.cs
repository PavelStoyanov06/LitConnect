namespace LitConnect.Services.Contracts;

using LitConnect.Services.Models;

public interface IDiscussionService
{
    Task<IEnumerable<DiscussionDto>> GetBookClubDiscussionsAsync(string bookClubId);

    Task<DiscussionDto?> GetDetailsAsync(string id, string userId);

    Task<string> CreateAsync(string title, string content, string bookClubId, string? bookId, string authorId);

    Task DeleteAsync(string id);

    Task<bool> CanUserDeleteAsync(string discussionId, string userId);

    Task<bool> ExistsAsync(string id);
}