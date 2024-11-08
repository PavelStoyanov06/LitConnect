namespace LitConnect.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(BookId), nameof(GenreId))]
public class BookGenre
{
    [Comment("ID of the book")]
    public string BookId { get; set; } = null!;

    [ForeignKey(nameof(BookId))]
    [Comment("Reference to the book")]
    public Book Book { get; set; } = null!;

    [Comment("ID of the genre")]
    public string GenreId { get; set; } = null!;

    [ForeignKey(nameof(GenreId))]
    [Comment("Reference to the genre")]
    public Genre Genre { get; set; } = null!;

    [Comment("Indicates if the book genre is deleted")]
    public bool IsDeleted { get; set; } = false;
}