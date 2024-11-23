namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Genre;
using Microsoft.EntityFrameworkCore;

public class GenreService : IGenreService
{
    private readonly LitConnectDbContext _context;

    public GenreService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GenreViewModel>> GetAllAsync()
    {
        return await _context.Genres
            .Where(g => !g.IsDeleted)
            .Select(g => new GenreViewModel
            {
                Id = g.Id,
                Name = g.Name,
                BooksCount = g.Books.Count
            })
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<GenreViewModel?> GetByIdAsync(string id)
    {
        return await _context.Genres
            .Where(g => g.Id == id && !g.IsDeleted)
            .Select(g => new GenreViewModel
            {
                Id = g.Id,
                Name = g.Name,
                BooksCount = g.Books.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Genres
            .AnyAsync(g => g.Id == id && !g.IsDeleted);
    }
}