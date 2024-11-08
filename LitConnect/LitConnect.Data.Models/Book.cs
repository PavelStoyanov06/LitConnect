namespace LitConnect.Data.Models;

using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Book : BaseModel
{
    public Book()
    {
        this.Reviews = new HashSet<Review>();
        this.BookClubs = new HashSet<BookClub>();
        this.ReadingLists = new HashSet<ReadingList>();
        this.Genres = new HashSet<BookGenre>();
    }

    [Required]
    [MinLength(ValidationConstants.Book.TitleMinLength)]
    [MaxLength(ValidationConstants.Book.TitleMaxLength)]
    [Comment("The title of the book")]
    public string Title { get; set; } = null!;

    [Required]
    [MinLength(ValidationConstants.Book.AuthorMinLength)]
    [MaxLength(ValidationConstants.Book.AuthorMaxLength)]
    [Comment("The author of the book")]
    public string Author { get; set; } = null!;

    [MaxLength(ValidationConstants.Book.DescriptionMaxLength)]
    [Comment("Brief description or summary of the book")]
    public string? Description { get; set; }

    [Comment("Collection of genres this book belongs to")]
    public ICollection<BookGenre> Genres { get; set; }

    [Comment("Collection of reviews associated with this book")]
    public ICollection<Review> Reviews { get; set; }

    [Comment("Collection of book clubs that have this book in their reading list")]
    public ICollection<BookClub> BookClubs { get; set; }

    [Comment("Collection of user reading lists that include this book")]
    public ICollection<ReadingList> ReadingLists { get; set; }
}