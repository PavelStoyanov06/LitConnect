namespace LitConnect.Services.Contracts;

using LitConnect.Web.ViewModels.Discussion;

public interface IDiscussionService
{
    Task<IEnumerable<DiscussionInListViewModel>> GetBookClubDiscussionsAsync(string bookClubId);

    Task<DiscussionDetailsViewModel?> GetDetailsAsync(string id, string userId);

    Task<string> CreateAsync(DiscussionCreateViewModel model, string authorId);

    Task DeleteAsync(string id);

    Task<bool> CanUserDeleteAsync(string discussionId, string userId);

    Task<bool> ExistsAsync(string id);
}