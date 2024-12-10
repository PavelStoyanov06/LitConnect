namespace LitConnect.Services.Models;

public class BookClubMembersDto
{
    public string BookClubId { get; set; } = null!;

    public string BookClubName { get; set; } = null!;

    public bool IsCurrentUserOwner { get; set; }

    public bool IsCurrentUserAdmin { get; set; }

    public ICollection<MemberDto> Members { get; set; } = new List<MemberDto>();
}