namespace LitConnect.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(BookClubId), nameof(UserId))]
public class UserBookClub
{
    [Comment("ID of the user who is a member")]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [Comment("ID of the book club")]
    public string BookClubId { get; set; } = null!;

    [ForeignKey(nameof(BookClubId))]
    [Comment("Reference to the book club")]
    public BookClub BookClub { get; set; } = null!;

    [Comment("Date when the user joined the book club")]
    public DateTime JoinedOn { get; set; }

    [Comment("Indicates if the user is an admin of the book club")]
    public bool IsAdmin { get; set; }

    [Comment("Indicates if the membership is deleted")]
    public bool IsDeleted { get; set; } = false;
}