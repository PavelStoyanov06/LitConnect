namespace LitConnect.Services.Models;

public class CommentDto
{
    public string Id { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string AuthorName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsCurrentUserAuthor { get; set; }
}