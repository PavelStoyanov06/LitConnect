using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LitConnect.Services.Tests;

[TestFixture]
public class GenreServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private GenreService genreService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectGenreTestDb_{Guid.NewGuid()}")
            .Options;

        this.dbContext = new LitConnectDbContext(this.dbOptions);
        this.genreService = new GenreService(this.dbContext);
    }

    [SetUp]
    public void Setup()
    {
        this.dbContext.Database.EnsureDeleted();
        this.dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedGenres()
    {
        await SeedDataAsync();

        var result = await genreService.GetAllAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ShouldReturnCorrectGenre()
    {
        await SeedDataAsync();
        var genreId = "genre1";

        var result = await genreService.GetByIdAsync(genreId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Fiction"));
        });
    }

    [Test]
    public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        await SeedDataAsync();
        var genreId = "genre1";

        var result = await genreService.ExistsAsync(genreId);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
    {
        await SeedDataAsync();
        var genreId = "nonexistent";

        var result = await genreService.ExistsAsync(genreId);

        Assert.That(result, Is.False);
    }

    private async Task SeedDataAsync()
    {
        var existingGenres = await dbContext.Genres.ToListAsync();
        dbContext.Genres.RemoveRange(existingGenres);
        await dbContext.SaveChangesAsync();

        var genres = new[]
        {
            new Genre
            {
                Id = "genre1",
                Name = "Fiction",
                IsDeleted = false
            },
            new Genre
            {
                Id = "genre2",
                Name = "Non-Fiction",
                IsDeleted = false
            },
            new Genre
            {
                Id = "genre3",
                Name = "Deleted Genre",
                IsDeleted = true
            }
        };

        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~GenreServiceTests()
    {
        Dispose(false);
    }
}