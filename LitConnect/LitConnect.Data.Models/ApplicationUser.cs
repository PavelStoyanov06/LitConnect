namespace LitConnect.Data.Models;

using LitConnect.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MinLength(ValidationConstants.User.FirstNameMinLength)]
    [MaxLength(ValidationConstants.User.FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(ValidationConstants.User.LastNameMinLength)]
    [MaxLength(ValidationConstants.User.LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    public bool IsDeleted { get; set; }
}