namespace LitConnect.Web.ViewModels.Book;

public class BookDetailsViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<string> Genres { get; set; } = new HashSet<string>();

    public int BookClubsCount { get; set; }
}