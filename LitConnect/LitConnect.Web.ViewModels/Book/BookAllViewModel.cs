namespace LitConnect.Web.ViewModels.Book;

public class BookAllViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public ICollection<string> Genres { get; set; } = new HashSet<string>();
}