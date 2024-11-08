namespace LitConnect.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(BookClubId), nameof(UserId))]
public class UserBookClub
{
    [Comment("ID of the user who is a member")]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [Comment("Reference to the member")]
    public ApplicationUser User { get; set; } = null!;

    [Comment("ID of the book club the user is a member of")]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    [Comment("Reference to the book club")]
    public BookClub BookClub { get; set; } = null!;

    [Comment("Date and time when the user joined or was approved to the book club")]
    public DateTime JoinedOn { get; set; }
}