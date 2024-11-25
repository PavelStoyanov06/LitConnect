namespace LitConnect.Web.ViewModels.Meeting;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.Meeting;

public class MeetingCreateViewModel
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength,
        ErrorMessage = "Title must be between {2} and {1} characters")]
    public string Title { get; set; } = null!;

    [StringLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Meeting Date and Time")]
    public DateTime ScheduledDate { get; set; }

    [Required]
    public string BookClubId { get; set; } = null!;

    public string? BookId { get; set; }
}