namespace LitConnect.Data.Models.Common;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public abstract class BaseModel
{
    [Key]
    [Comment("Primary key of the entity")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsDeleted { get; set; } = false;
}