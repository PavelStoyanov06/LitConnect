namespace LitConnect.Data.Models;

using LitConnect.Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ReadingList : BaseModel
{
    public ReadingList()
    {
        this.BookStatuses = new HashSet<BookReadingStatus>();
    }

    [Required]
    [Comment("ID of the user who owns this reading list")]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [Comment("Reference to the user who owns this reading list")]
    public virtual ApplicationUser User { get; set; } = null!;

    [Comment("Collection of book statuses in this reading list")]
    public virtual ICollection<BookReadingStatus> BookStatuses { get; set; }
}