namespace LitConnect.Services.Models;

public class BookDto
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Description { get; set; }

    public int BookClubsCount { get; set; }

    public ICollection<GenreDto> Genres { get; set; } = new HashSet<GenreDto>();
}