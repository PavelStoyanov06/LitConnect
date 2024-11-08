namespace LitConnect.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(BookClubId), nameof(UserId))]
public class UserBookClub
{
    [Comment("ID of the book club the user is a member of")]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    [Comment("ID of the book club the user is a member of")]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    [Comment("Reference to the book club")]
    public virtual BookClub BookClub { get; set; } = null!;

    [Comment("Date and time when the user joined or was approved to the book club")]
    public DateTime JoinedOn { get; set; }
}