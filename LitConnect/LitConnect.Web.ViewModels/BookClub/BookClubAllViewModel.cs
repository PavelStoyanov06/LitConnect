namespace LitConnect.Web.ViewModels.BookClub;

public class BookClubAllViewModel
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int MembersCount { get; set; }

    public bool IsUserMember { get; set; }
}