namespace LitConnect.Data.Models;

using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static LitConnect.Common.ValidationConstants.Meeting;

public class Meeting : BaseModel
{
    [Required]
    [MinLength(TitleMinLength)]
    [MaxLength(TitleMaxLength)]
    [Comment("Title or topic of the meeting")]
    public string Title { get; set; } = null!;

    [MaxLength(DescriptionMaxLength)]
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