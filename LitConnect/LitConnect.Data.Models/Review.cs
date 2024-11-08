namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Review : BaseModel
{
    [Required]
    [Range(ValidationConstants.Review.MinRating, ValidationConstants.Review.MaxRating)]
    public int Rating { get; set; }

    [MaxLength(ValidationConstants.Review.ContentMaxLength)]
    public string? Content { get; set; }

    [Required]
    public string BookId { get; set; } = null!;

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;

    [Required]
    public string ReviewerId { get; set; } = null!;

    [ForeignKey(nameof(ReviewerId))]
    public ApplicationUser Reviewer { get; set; } = null!;
}