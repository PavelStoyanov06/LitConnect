namespace LitConnect.Web.ViewModels.Discussion;

public class DiscussionInListViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string AuthorName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? BookTitle { get; set; }
}