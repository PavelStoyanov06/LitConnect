namespace LitConnect.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[PrimaryKey(nameof(BookClubId), nameof(UserId))]
public class UserBookClub
{
    public string UserId { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public string BookClubId { get; set; } = null!;

    public virtual BookClub BookClub { get; set; } = null!;

    public DateTime JoinedOn { get; set; }
}