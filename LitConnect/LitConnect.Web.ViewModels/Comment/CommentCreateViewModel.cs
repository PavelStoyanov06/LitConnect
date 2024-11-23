namespace LitConnect.Web.ViewModels.Comment;

using System.ComponentModel.DataAnnotations;

public class CommentCreateViewModel
{
    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = null!;

    public string DiscussionId { get; set; } = null!;
}