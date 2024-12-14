namespace LitConnect.Services.Implementations;

using LitConnect.Common;
using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

public class ReadingListService : IReadingListService
{
    private readonly LitConnectDbContext _context;

    public ReadingListService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<ReadingListDto> GetByUserIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found.");
        }

        var readingList = await _context.ReadingLists
            .Include(rl => rl.BookStatuses)
                .ThenInclude(bs => bs.Book)
                    .ThenInclude(b => b.Genres)
                        .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList == null)
        {
            readingList = new ReadingList
            {
                UserId = userId,
                BookStatuses = new HashSet<BookReadingStatus>()
            };
            _context.ReadingLists.Add(readingList);
            await _context.SaveChangesAsync();
        }

        return new ReadingListDto
        {
            Id = readingList.Id,
            UserId = readingList.UserId,
            UserName = $"{user.FirstName} {user.LastName}",
            Books = readingList.BookStatuses
                .Where(bs => !bs.Book.IsDeleted)
                .Select(bs => new ReadingListBookDto
                {
                    Id = bs.Book.Id,
                    Title = bs.Book.Title,
                    Author = bs.Book.Author,
                    Status = bs.Status,
                    Genres = bs.Book.Genres
                        .Where(bg => !bg.IsDeleted)
                        .Select(bg => bg.Genre.Name)
                })
                .ToList()
        };
    }

    public async Task AddBookAsync(string userId, string bookId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.BookStatuses)
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

        if (book != null && !readingList.BookStatuses.Any(bs => bs.BookId == bookId))
        {
            readingList.BookStatuses.Add(new BookReadingStatus
            {
                BookId = bookId,
                Status = ReadingStatus.WantToRead
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveBookAsync(string userId, string bookId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.BookStatuses)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList != null)
        {
            var bookStatus = readingList.BookStatuses.FirstOrDefault(bs => bs.BookId == bookId);
            if (bookStatus != null)
            {
                readingList.BookStatuses.Remove(bookStatus);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateBookStatusAsync(string userId, string bookId, ReadingStatus status)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.BookStatuses)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        if (readingList != null)
        {
            var bookStatus = readingList.BookStatuses.FirstOrDefault(bs => bs.BookId == bookId);
            if (bookStatus != null)
            {
                bookStatus.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<bool> HasBookAsync(string userId, string bookId)
    {
        var readingList = await _context.ReadingLists
            .Include(rl => rl.BookStatuses)
            .FirstOrDefaultAsync(rl => rl.UserId == userId && !rl.IsDeleted);

        return readingList?.BookStatuses.Any(bs => bs.BookId == bookId) ?? false;
    }
}