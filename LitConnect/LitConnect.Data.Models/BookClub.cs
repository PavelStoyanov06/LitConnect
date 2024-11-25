namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BookClub : BaseModel
{
    public BookClub()
    {
        this.Users = new HashSet<UserBookClub>();
        this.Discussions = new HashSet<Discussion>();
        this.Meetings = new HashSet<Meeting>();
        this.Books = new HashSet<Book>();
    }

    [Required]
    [MinLength(ValidationConstants.BookClub.NameMinLength)]
    [MaxLength(ValidationConstants.BookClub.NameMaxLength)]
    [Comment("The name of the book club")]
    public string Name { get; set; } = null!;

    [MaxLength(ValidationConstants.BookClub.DescriptionMaxLength)]
    [Comment("Description of the book club's focus, rules, or purpose")]
    public string? Description { get; set; }

    [Required]
    [Comment("ID of the user who created/owns the book club")]
    public string OwnerId { get; set; } = null!;

    [ForeignKey(nameof(OwnerId))]
    [Comment("Reference to the user who created/owns the book club")]
    public ApplicationUser Owner { get; set; } = null!;

    [Comment("Collection of user memberships in this book club")]
    public ICollection<UserBookClub> Users { get; set; }

    [Comment("Collection of discussions within this book club")]
    public ICollection<Discussion> Discussions { get; set; }

    [Comment("Collection of scheduled meetings for this book club")]
    public ICollection<Meeting> Meetings { get; set; }

    [Comment("Collection of books associated with this book club")]
    public ICollection<Book> Books { get; set; }

    [Comment("ID of the book currently being read")]
    public string? CurrentBookId { get; set; }
}