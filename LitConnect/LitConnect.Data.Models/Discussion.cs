namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Discussion : BaseModel
{
    [Required]
    [MinLength(ValidationConstants.Discussion.TitleMinLength)]
    [MaxLength(ValidationConstants.Discussion.TitleMaxLength)]
    [Comment("Title of the discussion thread")]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.Discussion.ContentMaxLength)]
    [Comment("Main content of the discussion post")]
    public string Content { get; set; } = null!;

    [Required]
    [Comment("ID of the user who started the discussion")]
    public string AuthorId { get; set; } = null!;

    [ForeignKey(nameof(AuthorId))]
    [Comment("Reference to the user who started the discussion")]
    public ApplicationUser Author { get; set; } = null!;

    [Required]
    [Comment("ID of the book club where this discussion takes place")]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    [Comment("Reference to the book club where this discussion takes place")]
    public BookClub BookClub { get; set; } = null!;

    [Comment("Optional ID of the book being discussed")]
    public string? BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    [Comment("Optional reference to the book being discussed")]
    public Book? Book { get; set; }
}