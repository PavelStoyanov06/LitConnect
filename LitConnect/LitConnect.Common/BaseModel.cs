namespace LitConnect.Data.Models.Common;

public abstract class BaseModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsDeleted { get; set; }
}