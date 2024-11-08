namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Discussion : BaseModel
{
    [Required]
    [MinLength(ValidationConstants.Discussion.TitleMinLength)]
    [MaxLength(ValidationConstants.Discussion.TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.Discussion.ContentMaxLength)]
    public string Content { get; set; } = null!;

    [Required]
    public string AuthorId { get; set; } = null!;

    [ForeignKey(nameof(AuthorId))]
    public ApplicationUser Author { get; set; } = null!;

    [Required]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    public BookClub BookClub { get; set; } = null!;

    public string? BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book? Book { get; set; }
}