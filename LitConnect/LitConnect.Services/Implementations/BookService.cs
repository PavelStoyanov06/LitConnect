namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Book;
using LitConnect.Web.ViewModels.Genre;
using Microsoft.EntityFrameworkCore;

public class BookService : IBookService
{
    private readonly LitConnectDbContext _context;

    public BookService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookAllViewModel>> GetAllAsync()
    {
        return await _context.Books
            .Where(b => !b.IsDeleted)
            .Select(b => new BookAllViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genres = b.Genres.Select(g => g.Genre.Name).ToList()
            })
            .ToListAsync();
    }

    public async Task<BookDetailsViewModel?> GetDetailsAsync(string id)
    {
        return await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .Select(b => new BookDetailsViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                Genres = b.Genres.Select(g => g.Genre.Name).ToList(),
                BookClubsCount = b.BookClubs.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(BookCreateViewModel model)
    {
        var book = new Book
        {
            Title = model.Title,
            Author = model.Author,
            Description = model.Description
        };

        foreach (var genreId in model.GenreIds)
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

    public async Task<IEnumerable<GenreViewModel>> GetAllGenresAsync()
    {
        return await _context.Genres
            .Where(g => !g.IsDeleted)
            .Select(g => new GenreViewModel
            {
                Id = g.Id,
                Name = g.Name,
                BooksCount = g.Books.Count
            })
            .ToListAsync();
    }

    public async Task AddToBookClubAsync(string bookId, string bookClubId)
    {
        var book = await _context.Books.FindAsync(bookId);
        var bookClub = await _context.BookClubs.FindAsync(bookClubId);

        if (book != null && bookClub != null)
        {
            bookClub.Books.Add(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromBookClubAsync(string bookId, string bookClubId)
    {
        var book = await _context.Books.FindAsync(bookId);
        var bookClub = await _context.BookClubs.FindAsync(bookClubId);

        if (book != null && bookClub != null)
        {
            bookClub.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Books.AnyAsync(b => b.Id == id && !b.IsDeleted);
    }
}