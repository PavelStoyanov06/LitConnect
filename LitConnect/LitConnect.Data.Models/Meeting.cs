namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Meeting : BaseModel
{
    [Required]
    [MinLength(ValidationConstants.Meeting.TitleMinLength)]
    [MaxLength(ValidationConstants.Meeting.TitleMaxLength)]
    [Comment("Title or topic of the meeting")]
    public string Title { get; set; } = null!;

    [MaxLength(ValidationConstants.Meeting.DescriptionMaxLength)]
    [Comment("Description of the meeting agenda or discussion points")]
    public string? Description { get; set; }

    [Required]
    [Comment("Date and time when the meeting is scheduled")]
    public DateTime ScheduledDate { get; set; }

    [Required]
    [Comment("ID of the book club hosting the meeting")]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    [Comment("Reference to the book club hosting the meeting")]
    public BookClub BookClub { get; set; } = null!;

    [Comment("Optional ID of the book to be discussed in the meeting")]
    public string? BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    [Comment("Optional reference to the book being discussed")]
    public Book? Book { get; set; }
}