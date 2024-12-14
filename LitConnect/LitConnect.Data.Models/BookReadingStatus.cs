namespace LitConnect.Data.Models;

using LitConnect.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(ReadingListId), nameof(BookId))]
public class BookReadingStatus
{
    [Required]
    [Comment("ID of the reading list")]
    public string ReadingListId { get; set; } = null!;

    [Required]
    [Comment("ID of the book")]
    public string BookId { get; set; } = null!;

    [Comment("Reading status of the book in the list")]
    public ReadingStatus Status { get; set; } = ReadingStatus.WantToRead;

    [ForeignKey(nameof(ReadingListId))]
    public virtual ReadingList ReadingList { get; set; } = null!;

    [ForeignKey(nameof(BookId))]
    public virtual Book Book { get; set; } = null!;
}