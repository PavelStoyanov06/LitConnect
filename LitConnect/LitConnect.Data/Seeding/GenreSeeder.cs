namespace LitConnect.Data.Seeding;

using LitConnect.Data.Models;

public class GenreSeeder
{
    public static async Task SeedAsync(LitConnectDbContext dbContext)
    {
        if (dbContext.Genres.Any())
        {
            return;
        }

        var genres = new List<Genre>()
        {
            new Genre { Name = "Fiction" },
            new Genre { Name = "Non-Fiction" },
            new Genre { Name = "Mystery" },
            new Genre { Name = "Science Fiction" },
            new Genre { Name = "Fantasy" },
            new Genre { Name = "Romance" },
            new Genre { Name = "Thriller" },
            new Genre { Name = "Horror" },
            new Genre { Name = "Historical Fiction" },
            new Genre { Name = "Biography" },
            new Genre { Name = "Self-Help" },
            new Genre { Name = "Business" },
            new Genre { Name = "Science" },
            new Genre { Name = "Poetry" }
        };

        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();
    }
}