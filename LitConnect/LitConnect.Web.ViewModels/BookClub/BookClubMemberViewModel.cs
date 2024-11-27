namespace LitConnect.Web.ViewModels.BookClub;

public class BookClubMemberViewModel
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime JoinedOn { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsOwner { get; set; }
}