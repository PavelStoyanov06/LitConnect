namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;

public class BookClub : BaseModel
{
    public BookClub()
    {
        this.Members = new HashSet<Member>();
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

    public virtual ApplicationUser Owner { get; set; } = null!;

    public virtual ICollection<Member> Members { get; set; }

    public virtual ICollection<Discussion> Discussions { get; set; }

    public virtual ICollection<Meeting> Meetings { get; set; }

    public virtual ICollection<Book> Books { get; set; }
}