namespace LitConnect.Data.Configurations.EntityConfigurations;

using LitConnect.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class BookConfiguration
{
    public static void ConfigureBook(this ModelBuilder builder)
    {
        // Seed Books
        builder.Entity<Book>().HasData(
            new Book
            {
                Id = "book_1",
                Title = "The Great Adventure",
                Author = "John Smith",
                Description = "An epic journey through unknown lands.",
                IsDeleted = false
            },
            new Book
            {
                Id = "book_2",
                Title = "Mystery at Midnight",
                Author = "Jane Doe",
                Description = "A thrilling detective story.",
                IsDeleted = false
            },
            new Book
            {
                Id = "book_3",
                Title = "The Future Now",
                Author = "Robert Johnson",
                Description = "A glimpse into tomorrow's world.",
                IsDeleted = false
            }
        );

        // Seed Book-Genre Relationships
        builder.Entity<BookGenre>().HasData(
            new BookGenre { BookId = "book_1", GenreId = "genre_fiction" },
            new BookGenre { BookId = "book_1", GenreId = "genre_fantasy" },
            new BookGenre { BookId = "book_2", GenreId = "genre_mystery" },
            new BookGenre { BookId = "book_2", GenreId = "genre_thriller" },
            new BookGenre { BookId = "book_3", GenreId = "genre_scifi" }
        );
    }
}