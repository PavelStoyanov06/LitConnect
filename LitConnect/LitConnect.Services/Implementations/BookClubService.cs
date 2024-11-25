namespace LitConnect.Services;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.BookClub;
using LitConnect.Web.ViewModels.Meeting;
using Microsoft.EntityFrameworkCore;

public class BookClubService : IBookClubService
{
    private readonly LitConnectDbContext _context;

    public BookClubService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookClubAllViewModel>> GetAllAsync(string userId)
    {
        return await _context.BookClubs
            .Where(bc => !bc.IsDeleted)
            .Select(bc => new BookClubAllViewModel
            {
                Id = bc.Id,
                Name = bc.Name,
                Description = bc.Description,
                MembersCount = bc.Users.Count,
                IsUserMember = bc.Users.Any(u => u.UserId == userId)
            })
            .ToListAsync();
    }

    public async Task<BookClubDetailsViewModel?> GetDetailsAsync(string bookClubId, string userId)
    {
        return await _context.BookClubs
            .Where(bc => bc.Id == bookClubId && !bc.IsDeleted)
            .Select(bc => new BookClubDetailsViewModel
            {
                Id = bc.Id,
                Name = bc.Name,
                Description = bc.Description,
                OwnerId = bc.OwnerId,
                OwnerName = $"{bc.Owner.FirstName} {bc.Owner.LastName}",
                MembersCount = bc.Users.Count,
                IsUserMember = bc.Users.Any(u => u.UserId == userId),
                IsUserOwner = bc.OwnerId == userId,
                Meetings = bc.Meetings
                .Where(m => !m.IsDeleted)
                .Select(m => new MeetingInListViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    ScheduledDate = m.ScheduledDate,
                    BookTitle = m.Book != null ? m.Book.Title : null
                })
                .ToList(),
                CurrentBook = bc.CurrentBookId != null ? new BookClubBookViewModel
                {
                    Id = bc.Books.First(b => b.Id == bc.CurrentBookId).Id,
                    Title = bc.Books.First(b => b.Id == bc.CurrentBookId).Title,
                    Author = bc.Books.First(b => b.Id == bc.CurrentBookId).Author,
                    IsCurrentlyReading = true,
                    Genres = bc.Books.First(b => b.Id == bc.CurrentBookId).Genres
                .Select(bg => bg.Genre.Name)
                .ToList()
                } : null,
                Books = bc.Books
                .Select(b => new BookClubBookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    IsCurrentlyReading = b.Id == bc.CurrentBookId,
                    Genres = b.Genres.Select(bg => bg.Genre.Name).ToList()
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(BookClubCreateViewModel model, string ownerId)
    {
        var bookClub = new BookClub
        {
            Name = model.Name,
            Description = model.Description,
            OwnerId = ownerId
        };

        await _context.BookClubs.AddAsync(bookClub);
        await _context.SaveChangesAsync();

        // Add owner as a member
        await JoinBookClubAsync(bookClub.Id, ownerId);

        return bookClub.Id;
    }

    public async Task JoinBookClubAsync(string bookClubId, string userId)
    {
        var isAlreadyMember = await _context.UsersBookClubs
            .AnyAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        if (!isAlreadyMember)
        {
            var membership = new UserBookClub
            {
                UserId = userId,
                BookClubId = bookClubId,
                JoinedOn = DateTime.UtcNow
            };

            await _context.UsersBookClubs.AddAsync(membership);
            await _context.SaveChangesAsync();
        }
    }

    public async Task LeaveBookClubAsync(string bookClubId, string userId)
    {
        var membership = await _context.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        if (membership != null)
        {
            _context.UsersBookClubs.Remove(membership);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddBookAsync(string bookClubId, string bookId, bool isCurrentlyReading)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            throw new InvalidOperationException("Book club not found");
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId && !b.IsDeleted);

        if (book == null)
        {
            throw new InvalidOperationException("Book not found");
        }

        var exists = await _context.BookClubs
            .AnyAsync(bc => bc.Id == bookClubId && bc.Books.Any(b => b.Id == bookId));

        if (!exists)
        {
            bookClub.Books.Add(book);

            if (isCurrentlyReading)
            {
                // First, remove current reading status from any other book
                var currentlyReadingBooks = await _context.BookClubs
                    .Where(bc => bc.Id == bookClubId && bc.CurrentBookId != null)
                    .ToListAsync();

                foreach (var bc in currentlyReadingBooks)
                {
                    bc.CurrentBookId = null;
                }

                // Set this book as currently reading
                bookClub.CurrentBookId = bookId;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveBookAsync(string bookClubId, string bookId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            throw new InvalidOperationException("Book club not found");
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId && !b.IsDeleted);

        if (book == null)
        {
            throw new InvalidOperationException("Book not found");
        }

        if (bookClub.CurrentBookId == bookId)
        {
            bookClub.CurrentBookId = null;
        }

        bookClub.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task SetCurrentlyReadingAsync(string bookClubId, string bookId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            throw new InvalidOperationException("Book club not found");
        }

        var hasBook = bookClub.Books.Any(b => b.Id == bookId);
        if (!hasBook)
        {
            throw new InvalidOperationException("Book is not in this book club");
        }

        // Remove current reading status from any other book
        bookClub.CurrentBookId = bookId;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookClubBookViewModel>> GetBooksAsync(string bookClubId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Books)
                .ThenInclude(b => b.Genres)
                    .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            throw new InvalidOperationException("Book club not found");
        }

        return bookClub.Books
            .Where(b => !b.IsDeleted)
            .Select(b => new BookClubBookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                IsCurrentlyReading = b.Id == bookClub.CurrentBookId,
                Genres = b.Genres
                    .Where(bg => !bg.IsDeleted)
                    .Select(bg => bg.Genre.Name)
                    .ToList()
            })
            .ToList();
    }

    public async Task<BookClubBookViewModel?> GetCurrentBookAsync(string bookClubId)
    {
        return await _context.BookClubs
            .Where(bc => bc.Id == bookClubId && !bc.IsDeleted && bc.CurrentBookId != null)
            .Select(bc => new BookClubBookViewModel
            {
                Id = bc.Books.First(b => b.Id == bc.CurrentBookId).Id,
                Title = bc.Books.First(b => b.Id == bc.CurrentBookId).Title,
                Author = bc.Books.First(b => b.Id == bc.CurrentBookId).Author,
                IsCurrentlyReading = true,
                Genres = bc.Books.First(b => b.Id == bc.CurrentBookId).Genres
                    .Where(bg => !bg.IsDeleted)
                    .Select(bg => bg.Genre.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }
}