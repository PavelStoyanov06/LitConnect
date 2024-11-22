namespace LitConnect.Web.ViewModels.Book;

using System.ComponentModel.DataAnnotations;
using static LitConnect.Common.ValidationConstants.Book;

public class BookCreateViewModel
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength,
        ErrorMessage = "Title must be between {2} and {1} characters")]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(AuthorMaxLength, MinimumLength = AuthorMinLength,
        ErrorMessage = "Author name must be between {2} and {1} characters")]
    public string Author { get; set; } = null!;

    [StringLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [Display(Name = "Genres")]
    public required IEnumerable<string> GenreIds { get; set; }
}