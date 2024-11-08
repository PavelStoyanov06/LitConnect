namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BookClub : BaseModel
{
    public BookClub()
    {
        this.UsersBookClubs = new HashSet<UserBookClub>();
        this.Discussions = new HashSet<Discussion>();
        this.Meetings = new HashSet<Meeting>();
        this.Books = new HashSet<Book>();
    }

    [Required]
    [MinLength(ValidationConstants.BookClub.NameMinLength)]
    [MaxLength(ValidationConstants.BookClub.NameMaxLength)]
    public string Name { get; set; } = null!;

    [MaxLength(ValidationConstants.BookClub.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    public string OwnerId { get; set; } = null!;

    [ForeignKey(nameof(OwnerId))]
    public ApplicationUser Owner { get; set; } = null!;

    public ICollection<UserBookClub> UsersBookClubs { get; set; }

    public ICollection<Discussion> Discussions { get; set; }

    public ICollection<Meeting> Meetings { get; set; }

    public ICollection<Book> Books { get; set; }
}