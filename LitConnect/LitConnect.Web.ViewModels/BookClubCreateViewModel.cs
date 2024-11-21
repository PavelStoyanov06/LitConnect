namespace LitConnect.Web.ViewModels.BookClub;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.BookClub;

public class BookClubCreateViewModel
{
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength,
        ErrorMessage = "Name must be between {2} and {1} characters")]
    [Display(Name = "Club Name")]
    public string Name { get; set; } = null!;

    [StringLength(DescriptionMaxLength)]
    public string? Description { get; set; }
}