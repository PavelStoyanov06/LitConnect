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
    [ForeignKey(nameof(Author))]
    public string AuthorId { get; set; } = null!;

    public virtual ApplicationUser Author { get; set; } = null!;

    [Required]
    public string BookClubId { get; set; } = null!;

    public virtual BookClub BookClub { get; set; } = null!;

    public string? BookId { get; set; }

    public virtual Book? Book { get; set; }
}