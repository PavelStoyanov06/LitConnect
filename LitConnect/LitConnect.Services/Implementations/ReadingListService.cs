namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

public class ReadingListService : IReadingListService
{
    private readonly LitConnectDbContext _context;
    private readonly Dictionary<string, ReadingStatus> _bookStatuses;

    public ReadingListService(LitConnectDbContext context)
    {
        _context = context;
        _bookStatuses = new Dictionary<string, ReadingStatus>();
    }

    public async Task<ReadingListDto> GetByUserIdAsync(string userId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.User)
            .Include(rl => rl.Books)
                .ThenInclude(b => b.Genres)
                    .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList == null)
        {
            readingList = new ReadingList
            {
                UserId = userId
            };
            await _context.ReadingLists.AddAsync(readingList);
            await _context.SaveChangesAsync();

            readingList = await _context.ReadingLists
                .Include(rl => rl.User)
                .Include(rl => rl.Books)
                    .ThenInclude(b => b.Genres)
                        .ThenInclude(bg => bg.Genre)
                .FirstAsync(rl => rl.Id == readingList.Id);
        }

        return new ReadingListDto
        {
            Id = readingList.Id,
            UserId = readingList.UserId,
            UserName = $"{readingList.User.FirstName} {readingList.User.LastName}",
            Books = readingList.Books
                .Where(b => !b.IsDeleted)
                .Select(b => new ReadingListBookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Status = GetBookStatus(readingList.Id, b.Id),
                    Genres = b.Genres
                        .Where(bg => !bg.IsDeleted)
                        .Select(bg => bg.Genre.Name)
                })
                .ToList()
        };
    }

    public async Task AddBookAsync(string userId, string bookId)
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
        }

        var book = await _context.Books.FindAsync(bookId);

        if (book != null && !readingList.Books.Any(b => b.Id == bookId))
        {
            readingList.Books.Add(book);
            SetBookStatus(readingList.Id, bookId, ReadingStatus.WantToRead);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveBookAsync(string userId, string bookId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.Books)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList != null)
        {
            var book = readingList.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                readingList.Books.Remove(book);
                RemoveBookStatus(readingList.Id, bookId);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateBookStatusAsync(string userId, string bookId, ReadingStatus status)
    {
        var readingList = await _context.ReadingLists
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList != null)
        {
            var key = $"{readingList.Id}_{bookId}";
            _bookStatuses[key] = status;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasBookAsync(string userId, string bookId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.Books)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        return readingList?.Books.Any(b => b.Id == bookId) ?? false;
    }

    private ReadingStatus GetBookStatus(string readingListId, string bookId)
    {
        var key = $"{readingListId}_{bookId}";
        return _bookStatuses.GetValueOrDefault(key, ReadingStatus.WantToRead);
    }

    private void SetBookStatus(string readingListId, string bookId, ReadingStatus status)
    {
        var key = $"{readingListId}_{bookId}";
        _bookStatuses[key] = status;
    }

    private void RemoveBookStatus(string readingListId, string bookId)
    {
        var key = $"{readingListId}_{bookId}";
        _bookStatuses.Remove(key);
    }
}