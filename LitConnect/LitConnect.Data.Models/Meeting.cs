namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Meeting : BaseModel
{
    [Required]
    [MinLength(ValidationConstants.Meeting.TitleMinLength)]
    [MaxLength(ValidationConstants.Meeting.TitleMaxLength)]
    public string Title { get; set; } = null!;

    [MaxLength(ValidationConstants.Meeting.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [Required]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    public BookClub BookClub { get; set; } = null!;

    public string? BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book? Book { get; set; }
}