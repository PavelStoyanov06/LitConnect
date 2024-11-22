namespace LitConnect.Services;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.BookClub;
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
                IsUserOwner = bc.OwnerId == userId
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
}