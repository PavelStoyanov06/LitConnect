using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace LitConnect.Services.Implementations;

public class BookService : IBookService
{
    private readonly LitConnectDbContext _context;

    public BookService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync()
    {
        return await _context.Books
            .Where(b => !b.IsDeleted)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                BookClubsCount = b.BookClubs.Count,
                Genres = b.Genres
                    .Where(bg => !bg.IsDeleted)
                    .Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        BooksCount = bg.Genre.Books.Count(b => !b.IsDeleted)
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<BookDto?> GetByIdAsync(string id)
    {
        return await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                BookClubsCount = b.BookClubs.Count,
                Genres = b.Genres
                    .Where(bg => !bg.IsDeleted)
                    .Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        BooksCount = bg.Genre.Books.Count(b => !b.IsDeleted)
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(string title, string author, string? description, IEnumerable<string> genreIds)
    {
        var book = new Book
        {
            Title = title,
            Author = author,
            Description = description
        };

        foreach (var genreId in genreIds)
        {
            book.Genres.Add(new BookGenre
            {
                GenreId = genreId
            });
        }

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        return book.Id;
    }

    public async Task DeleteAsync(string id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var bookClubsWithThisBook = await _context.BookClubs
                .Where(bc => bc.CurrentBookId == id)
                .ToListAsync();

            foreach (var bookClub in bookClubsWithThisBook)
            {
                bookClub.CurrentBookId = null;
            }

            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.IsDeleted = true;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Books
            .AnyAsync(b => b.Id == id && !b.IsDeleted);
    }

    public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
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
}