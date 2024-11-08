namespace LitConnect.Web.ViewModels.User;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.User;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength,
        ErrorMessage = "First name must be between {2} and {1} characters")]
    [Display(Name = "First Name")]
    public required string FirstName { get; init; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength,
        ErrorMessage = "Last name must be between {2} and {1} characters")]
    [Display(Name = "Last Name")]
    public required string LastName { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "Password must be between {2} and {1} characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; init; }

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match")]
    public required string ConfirmPassword { get; init; }
}