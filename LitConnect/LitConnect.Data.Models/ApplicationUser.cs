namespace LitConnect.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.User;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        this.BookClubs = new HashSet<UserBookClub>();
    }

    [Required]
    [MinLength(FirstNameMinLength)]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(LastNameMinLength)]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [Comment("Book clubs where the user is a member")]
    public ICollection<UserBookClub> BookClubs { get; set; }

    public bool IsDeleted { get; set; } = false;
}