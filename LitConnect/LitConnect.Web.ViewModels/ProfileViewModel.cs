namespace LitConnect.Web.ViewModels.User;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.User;

public class ProfileViewModel
{
    public string Id { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(FirstNameMinLength)]
    [MaxLength(FirstNameMaxLength)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(LastNameMinLength)]
    [MaxLength(LastNameMaxLength)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Book Clubs")]
    public int BookClubsCount { get; set; }
}