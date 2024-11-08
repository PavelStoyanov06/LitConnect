namespace LitConnect.Data.Models.Common;

using System.ComponentModel.DataAnnotations;

public abstract class BaseModel
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsDeleted { get; set; } = false;
}