namespace LitConnect.Web.ViewModels.User;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.User;

public class ProfileViewModel
{
    public required string Id { get; set; }

    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength,
        ErrorMessage = "First name must be between {2} and {1} characters")]
    [Display(Name = "First Name")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength,
        ErrorMessage = "Last name must be between {2} and {1} characters")]
    [Display(Name = "Last Name")]
    public required string LastName { get; set; }

    [Display(Name = "Book Clubs")]
    public int BookClubsCount { get; set; }
}