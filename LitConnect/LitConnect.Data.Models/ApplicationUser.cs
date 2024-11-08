namespace LitConnect.Data.Models;

using LitConnect.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        this.BookClubs = new HashSet<UserBookClub>();
    }

    [Required]
    [MinLength(ValidationConstants.User.FirstNameMinLength)]
    [MaxLength(ValidationConstants.User.FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(ValidationConstants.User.LastNameMinLength)]
    [MaxLength(ValidationConstants.User.LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [Comment("Book clubs where the user is a member")]
    public ICollection<UserBookClub> BookClubs { get; set; }

    public bool IsDeleted { get; set; } = false;
}