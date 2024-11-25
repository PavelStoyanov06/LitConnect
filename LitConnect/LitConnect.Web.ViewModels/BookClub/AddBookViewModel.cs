namespace LitConnect.Web.ViewModels.BookClub;

public class AddBookViewModel
{
    public string BookClubId { get; set; } = null!;

    public required string BookId { get; set; }

    public bool IsCurrentlyReading { get; set; }
}