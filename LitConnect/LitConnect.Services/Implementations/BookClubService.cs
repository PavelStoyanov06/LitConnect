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
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId);

        var book = await _context.Books.FindAsync(bookId);

        if (bookClub != null && book != null)
        {
            bookClub.Books.Add(book);

            if (isCurrentlyReading)
            {
                // Logic for marking as currently reading
                // You might need to add a property to track this
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveBookAsync(string bookClubId, string bookId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Books)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId);

        var book = await _context.Books.FindAsync(bookId);

        if (bookClub != null && book != null)
        {
            bookClub.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetCurrentlyReadingAsync(string bookClubId, string bookId)
    {
        // Implement logic for setting current book
        // You might need to add a property or table to track this
    }

    public async Task<IEnumerable<BookClubBookViewModel>> GetBooksAsync(string bookClubId)
    {
        return await _context.BookClubs
            .Where(bc => bc.Id == bookClubId)
            .SelectMany(bc => bc.Books)
            .Select(b => new BookClubBookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genres = b.Genres.Select(g => g.Genre.Name),
                IsCurrentlyReading = false // You'll need to implement this logic
            })
            .ToListAsync();
    }
}