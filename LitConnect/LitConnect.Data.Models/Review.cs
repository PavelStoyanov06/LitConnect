namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Review : BaseModel
{
    [Required]
    [Range(ValidationConstants.Review.MinRating, ValidationConstants.Review.MaxRating)]
    [Comment("Rating given to the book (1-5 stars)")]
    public int Rating { get; set; }

    [MaxLength(ValidationConstants.Review.ContentMaxLength)]
    [Comment("Written review content explaining the rating")]
    public string? Content { get; set; }

    [Required]
    [Comment("ID of the book being reviewed")]
    public string BookId { get; set; } = null!;

    [ForeignKey(nameof(BookId))]
    [Comment("Reference to the book being reviewed")]
    public Book Book { get; set; } = null!;

    [Required]
    [Comment("ID of the user who wrote the review")]
    public string ReviewerId { get; set; } = null!;

    [ForeignKey(nameof(ReviewerId))]
    [Comment("Reference to the user who wrote the review")]
    public ApplicationUser Reviewer { get; set; } = null!;
}