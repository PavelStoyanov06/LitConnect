namespace LitConnect.Web.ViewModels.BookClub;

public class BookClubMembersViewModel
{
    public string BookClubId { get; set; } = null!;

    public string BookClubName { get; set; } = null!;

    public bool IsCurrentUserOwner { get; set; }

    public bool IsCurrentUserAdmin { get; set; }

    public IEnumerable<BookClubMemberViewModel> Members { get; set; }
        = new List<BookClubMemberViewModel>();
}