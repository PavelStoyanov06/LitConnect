namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comment : BaseModel
{
    [Required]
    [MaxLength(ValidationConstants.Comment.ContentMaxLength)]
    [Comment("Main content of the comment")]
    public string Content { get; set; } = null!;

    [Required]
    [Comment("ID of the user who wrote the comment")]
    public string AuthorId { get; set; } = null!;

    [ForeignKey(nameof(AuthorId))]
    [Comment("Reference to the user who wrote the comment")]
    public ApplicationUser Author { get; set; } = null!;

    [Required]
    [Comment("ID of the discussion this comment belongs to")]
    public string DiscussionId { get; set; } = null!;

    [ForeignKey(nameof(DiscussionId))]
    [Comment("Reference to the discussion")]
    public Discussion Discussion { get; set; } = null!;

    [Comment("Date and time when the comment was created")]
    public DateTime CreatedOn { get; set; }
}