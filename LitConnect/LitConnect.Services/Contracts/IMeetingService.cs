using LitConnect.Services.Models;

namespace LitConnect.Services.Contracts;

public interface IMeetingService
{
    Task<IEnumerable<MeetingDto>> GetBookClubMeetingsAsync(string bookClubId);

    Task<MeetingDto?> GetByIdAsync(string id);

    Task<string> CreateAsync(string title, string? description, DateTime scheduledDate, string bookClubId, string? bookId);

    Task DeleteAsync(string id);

    Task<bool> ExistsAsync(string id);

    Task<MeetingDto?> GetDetailsAsync(string id);
}