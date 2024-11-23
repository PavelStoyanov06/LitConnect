using LitConnect.Web.ViewModels.Discussion;

public class BookClubDetailsViewModel
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string OwnerId { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public int MembersCount { get; set; }

    public bool IsUserMember { get; set; }

    public bool IsUserOwner { get; set; }

    public IEnumerable<DiscussionInListViewModel> Discussions { get; set; }
        = new List<DiscussionInListViewModel>();
}