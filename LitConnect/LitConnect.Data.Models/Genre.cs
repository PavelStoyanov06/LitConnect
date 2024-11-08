namespace LitConnect.Data.Models;

using System.ComponentModel.DataAnnotations;
using LitConnect.Common;
using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;

public class Genre : BaseModel
{
    public Genre()
    {
        this.Books = new HashSet<BookGenre>();
    }

    [Required]
    [MaxLength(ValidationConstants.Book.GenreMaxLength)]
    [Comment("Name of the genre")]
    public string Name { get; set; } = null!;

    [Comment("Collection of books in this genre")]
    public ICollection<BookGenre> Books { get; set; }
}