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
    [ForeignKey(nameof(BookClub))]
    public string BookClubId { get; set; } = null!;

    public BookClub BookClub { get; set; } = null!;

    [ForeignKey(nameof(Book))]
    public string? BookId { get; set; }

    public Book? Book { get; set; }
}