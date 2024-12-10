namespace LitConnect.Services.Models;

public class DiscussionDto
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string AuthorName { get; set; } = null!;

    public string BookClubId { get; set; } = null!;

    public string BookClubName { get; set; } = null!;

    public string? BookTitle { get; set; }

    public DateTime CreatedOn { get; set; }

    public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
}