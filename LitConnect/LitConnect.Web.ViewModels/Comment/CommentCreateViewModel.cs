namespace LitConnect.Web.ViewModels.Comment;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.Comment;

public class CommentCreateViewModel
{
    [Required]
    [StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
    public string Content { get; set; } = null!;

    public string DiscussionId { get; set; } = null!;
}