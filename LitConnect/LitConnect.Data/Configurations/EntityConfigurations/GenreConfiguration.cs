namespace LitConnect.Data.Configurations.EntityConfigurations;

using LitConnect.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class GenreConfiguration
{
    public static void ConfigureGenre(this ModelBuilder builder)
    {
        builder.Entity<Genre>().HasData(
            new Genre { Id = "genre_fiction", Name = "Fiction" },
            new Genre { Id = "genre_nonfiction", Name = "Non-Fiction" },
            new Genre { Id = "genre_mystery", Name = "Mystery" },
            new Genre { Id = "genre_scifi", Name = "Science Fiction" },
            new Genre { Id = "genre_fantasy", Name = "Fantasy" },
            new Genre { Id = "genre_romance", Name = "Romance" },
            new Genre { Id = "genre_thriller", Name = "Thriller" },
            new Genre { Id = "genre_horror", Name = "Horror" },
            new Genre { Id = "genre_historical", Name = "Historical Fiction" },
            new Genre { Id = "genre_biography", Name = "Biography" }
        );
    }
}