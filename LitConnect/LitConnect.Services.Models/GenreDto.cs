namespace LitConnect.Services.Models;

public class GenreDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int BooksCount { get; set; }
}