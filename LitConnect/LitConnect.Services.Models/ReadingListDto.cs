namespace LitConnect.Services.Models;

public class ReadingListDto
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public ICollection<ReadingListBookDto> Books { get; set; } = new List<ReadingListBookDto>();
}