namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.ReadingList;
using Microsoft.EntityFrameworkCore;

public class ReadingListService : IReadingListService
{
    private readonly LitConnectDbContext _context;

    public ReadingListService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<ReadingListViewModel> GetByUserIdAsync(string userId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.User)
            .Include(rl => rl.Books)
                .ThenInclude(b => b.Genres)
                    .ThenInclude(bg => bg.Genre)
            .Where(rl => rl.UserId == userId && !rl.IsDeleted)
            .FirstOrDefaultAsync() ?? new ReadingList
            {
                UserId = userId,
                Books = new HashSet<Book>(),
                User = await _context.Users.FirstAsync(u => u.Id == userId)
            };

        if (readingList == null)
        {
            readingList = new ReadingList
            {
                UserId = userId
            };
            await _context.ReadingLists.AddAsync(readingList);
            await _context.SaveChangesAsync();
        }

        return new ReadingListViewModel
        {
            Id = readingList.Id,
            UserId = readingList.UserId,
            UserName = $"{readingList.User.FirstName} {readingList.User.LastName}",
            BooksCount = readingList.Books.Count,
            Books = readingList.Books
                .Select(b => new ReadingListBookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Status = ReadingStatus.WantToRead, // You'll need to add status tracking
                    Genres = b.Genres.Select(bg => bg.Genre.Name)
                })
                .ToList()
        };
    }

    public async Task AddBookAsync(string userId, string bookId)
    {
        var readingList = await GetOrCreateReadingListAsync(userId);
        var book = await _context.Books.FindAsync(bookId);

        if (book != null && !readingList.Books.Any(b => b.Id == bookId))
        {
            readingList.Books.Add(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveBookAsync(string userId, string bookId)
    {
        var readingList = await GetOrCreateReadingListAsync(userId);
        var book = readingList.Books.FirstOrDefault(b => b.Id == bookId);

        if (book != null)
        {
            readingList.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateBookStatusAsync(string userId, string bookId, ReadingStatus status)
    {
        // You'll need to add status tracking to implement this
        throw new NotImplementedException();
    }

    public async Task<bool> HasBookAsync(string userId, string bookId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.Books)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        return readingList?.Books.Any(b => b.Id == bookId) ?? false;
    }

    private async Task<ReadingList> GetOrCreateReadingListAsync(string userId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.Books)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList == null)
        {
            readingList = new ReadingList
            {
                UserId = userId
            };
            await _context.ReadingLists.AddAsync(readingList);
            await _context.SaveChangesAsync();
        }

        return readingList;
    }
}