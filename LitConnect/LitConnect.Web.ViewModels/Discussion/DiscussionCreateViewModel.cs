namespace LitConnect.Web.ViewModels.Discussion;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.Discussion;

public class DiscussionCreateViewModel
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength,
        ErrorMessage = "Title must be between {2} and {1} characters")]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(ContentMaxLength, MinimumLength = 1,
        ErrorMessage = "Content cannot be empty and must not exceed {1} characters")]
    public string Content { get; set; } = null!;

    [Required]
    public string BookClubId { get; set; } = null!;

    public string? BookId { get; set; }
}