namespace LitConnect.Web.ViewModels.Genre;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.Book;

public class GenreViewModel
{
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(GenreMaxLength, MinimumLength = GenreMinLength)]
    public string Name { get; set; } = null!;

    public int BooksCount { get; set; }
}