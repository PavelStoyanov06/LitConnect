namespace LitConnect.Web.ViewModels.Discussion;

using LitConnect.Web.ViewModels.Comment;

public class DiscussionDetailsViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string AuthorName { get; set; } = null!;

    public string BookClubId { get; set; } = null!;

    public string BookClubName { get; set; } = null!;

    public string? BookTitle { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsCurrentUserAuthor { get; set; }

    public bool IsCurrentUserAdmin { get; set; }

    public bool IsCurrentUserOwner { get; set; }

    public bool CanDelete => IsCurrentUserAuthor || IsCurrentUserAdmin || IsCurrentUserOwner;

    public IEnumerable<CommentViewModel> Comments { get; set; }
        = new List<CommentViewModel>();

    public CommentCreateViewModel NewComment { get; set; } = null!;
}