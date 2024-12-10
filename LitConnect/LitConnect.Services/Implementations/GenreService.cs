namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

public class GenreService : IGenreService
{
    private readonly LitConnectDbContext _context;

    public GenreService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        return await _context.Genres
            .Where(g => !g.IsDeleted)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                BooksCount = g.Books.Count(b => !b.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<GenreDto?> GetByIdAsync(string id)
    {
        return await _context.Genres
            .Where(g => g.Id == id && !g.IsDeleted)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                BooksCount = g.Books.Count(b => !b.IsDeleted)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Genres
            .AnyAsync(g => g.Id == id && !g.IsDeleted);
    }

    public async Task<int> GetBooksCountAsync(string id)
    {
        return await _context.BooksGenres
            .CountAsync(bg => bg.GenreId == id && !bg.IsDeleted);
    }
}