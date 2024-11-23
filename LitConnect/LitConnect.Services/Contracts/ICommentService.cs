using LitConnect.Web.ViewModels.Comment;

public interface ICommentService
{
    Task<string> CreateAsync(CommentCreateViewModel model, string authorId);
    Task DeleteAsync(string id);
    Task<bool> IsUserAuthorAsync(string commentId, string userId);
}