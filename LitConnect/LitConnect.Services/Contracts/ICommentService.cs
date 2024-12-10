namespace LitConnect.Services.Contracts;

public interface ICommentService
{
    Task<string> CreateAsync(string content, string discussionId, string authorId);

    Task DeleteAsync(string id);

    Task<bool> IsUserAuthorAsync(string commentId, string userId);
}