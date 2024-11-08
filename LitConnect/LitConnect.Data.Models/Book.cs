namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;

public class Book : BaseModel
{
    public Book()
    {
        this.Reviews = new HashSet<Review>();
        this.BookClubs = new HashSet<BookClub>();
        this.ReadingLists = new HashSet<ReadingList>();
    }

    [Required]
    [MinLength(ValidationConstants.Book.TitleMinLength)]
    [MaxLength(ValidationConstants.Book.TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Required]
    [MinLength(ValidationConstants.Book.AuthorMinLength)]
    [MaxLength(ValidationConstants.Book.AuthorMaxLength)]
    public string Author { get; set; } = null!;

    [MaxLength(ValidationConstants.Book.DescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(ValidationConstants.Book.GenreMaxLength)]
    public string Genre { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; }

    public ICollection<BookClub> BookClubs { get; set; }

    public ICollection<ReadingList> ReadingLists { get; set; }
}