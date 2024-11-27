namespace LitConnect.Web.ViewModels.ReadingList;

public class ReadingListBookViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public ReadingStatus Status { get; set; }

    public IEnumerable<string> Genres { get; set; } = new List<string>();
}