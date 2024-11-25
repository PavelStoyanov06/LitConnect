namespace LitConnect.Web.ViewModels.BookClub;

public class BookClubBookViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public bool IsCurrentlyReading { get; set; }

    public IEnumerable<string> Genres { get; set; } = new List<string>();
}