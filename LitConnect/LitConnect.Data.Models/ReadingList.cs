namespace LitConnect.Data.Models;

using LitConnect.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ReadingList : BaseModel
{
    public ReadingList()
    {
        this.Books = new HashSet<Book>();
    }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; }
}