namespace LitConnect.Web.ViewModels.ReadingList;

public class ReadingListViewModel
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int BooksCount { get; set; }

    public IEnumerable<ReadingListBookViewModel> Books { get; set; }
        = new List<ReadingListBookViewModel>();
}
