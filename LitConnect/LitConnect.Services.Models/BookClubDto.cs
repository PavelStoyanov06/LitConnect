namespace LitConnect.Services.Models;

public class BookClubDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string OwnerId { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public int MembersCount { get; set; }

    public bool IsUserMember { get; set; }

    public bool IsUserOwner { get; set; }

    public bool IsUserAdmin { get; set; }

    public ICollection<MemberDto> Members { get; set; } = new List<MemberDto>();

    public BookDto? CurrentBook { get; set; }

    public ICollection<BookDto> Books { get; set; } = new List<BookDto>();

    public ICollection<DiscussionDto> Discussions { get; set; } = new List<DiscussionDto>();

    public ICollection<MeetingDto> Meetings { get; set; } = new List<MeetingDto>();
}