namespace LitConnect.Services.Contracts;

using LitConnect.Web.ViewModels.Meeting;

public interface IMeetingService
{
    Task<IEnumerable<MeetingInListViewModel>> GetBookClubMeetingsAsync(string bookClubId);

    Task<MeetingDetailsViewModel?> GetDetailsAsync(string id);

    Task<string> CreateAsync(MeetingCreateViewModel model);

    Task DeleteAsync(string id);

    Task<bool> ExistsAsync(string id);
}