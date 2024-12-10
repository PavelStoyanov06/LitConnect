using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace LitConnect.Services.Implementations;

public class BookClubService : IBookClubService
{
    private readonly LitConnectDbContext _context;

    public BookClubService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<BookClubDto?> GetByIdAsync(string id)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Owner)
            .FirstOrDefaultAsync(bc => bc.Id == id && !bc.IsDeleted);

        if (bookClub == null)
        {
            return null;
        }

        return new BookClubDto
        {
            Id = bookClub.Id,
            Name = bookClub.Name,
            Description = bookClub.Description,
            OwnerId = bookClub.OwnerId,
            OwnerName = $"{bookClub.Owner.FirstName} {bookClub.Owner.LastName}"
        };
    }

    public async Task<IEnumerable<BookClubDto>> GetAllAsync()
    {
        return await _context.BookClubs
            .Where(bc => !bc.IsDeleted)
            .Select(bc => new BookClubDto
            {
                Id = bc.Id,
                Name = bc.Name,
                Description = bc.Description,
                OwnerId = bc.OwnerId,
                OwnerName = $"{bc.Owner.FirstName} {bc.Owner.LastName}",
                MembersCount = bc.Users.Count(u => !u.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync(string bookClubId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Users)
                .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            throw new InvalidOperationException("Book club not found");
        }

        return bookClub.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new MemberDto
            {
                UserId = u.UserId,
                UserName = $"{u.User.FirstName} {u.User.LastName}",
                Email = u.User.Email!,
                JoinedOn = u.JoinedOn,
                IsAdmin = u.IsAdmin,
                IsOwner = bookClub.OwnerId == u.UserId
            });
    }

    public async Task<BookClubDto?> GetDetailsAsync(string id, string userId)
    {
        return await _context.BookClubs
            .Where(bc => bc.Id == id && !bc.IsDeleted)
            .Select(bc => new BookClubDto
            {
                Id = bc.Id,
                Name = bc.Name,
                Description = bc.Description,
                OwnerId = bc.OwnerId,
                OwnerName = $"{bc.Owner.FirstName} {bc.Owner.LastName}",
                MembersCount = bc.Users.Count(u => !u.IsDeleted),
                IsUserMember = bc.Users.Any(u => u.UserId == userId && !u.IsDeleted),
                IsUserOwner = bc.OwnerId == userId,
                IsUserAdmin = bc.Users.Any(u => u.UserId == userId && u.IsAdmin && !u.IsDeleted),
                CurrentBook = bc.CurrentBookId != null ? new BookDto
                {
                    Id = bc.Books.First(b => b.Id == bc.CurrentBookId).Id,
                    Title = bc.Books.First(b => b.Id == bc.CurrentBookId).Title,
                    Author = bc.Books.First(b => b.Id == bc.CurrentBookId).Author
                } : null,
                Books = bc.Books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Description = b.Description,
                    Genres = b.Genres
                        .Where(bg => !bg.IsDeleted)
                        .Select(bg => new GenreDto
                        {
                            Id = bg.Genre.Id,
                            Name = bg.Genre.Name,
                            BooksCount = bg.Genre.Books.Count(b => !b.IsDeleted)
                        }).ToList()
                }).ToList(),
                Members = bc.Users
                    .Where(u => !u.IsDeleted)
                    .Select(u => new MemberDto
                    {
                        UserId = u.UserId,
                        UserName = $"{u.User.FirstName} {u.User.LastName}",
                        Email = u.User.Email!,
                        JoinedOn = u.JoinedOn,
                        IsAdmin = u.IsAdmin,
                        IsOwner = bc.OwnerId == u.UserId
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(string name, string description, string ownerId)
    {
        var bookClub = new BookClub
        {
            Name = name,
            Description = description,
            OwnerId = ownerId
        };

        await _context.BookClubs.AddAsync(bookClub);
        await _context.SaveChangesAsync();

        await JoinBookClubAsync(bookClub.Id, ownerId);
        await SetAdminStatusAsync(bookClub.Id, ownerId, ownerId, true);

        return bookClub.Id;
    }

    public async Task JoinBookClubAsync(string bookClubId, string userId)
    {
        var existingMembership = await _context.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        if (existingMembership != null)
        {
            if (existingMembership.IsDeleted)
            {
                existingMembership.IsDeleted = false;
                existingMembership.JoinedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return;
        }

        var membership = new UserBookClub
        {
            UserId = userId,
            BookClubId = bookClubId,
            JoinedOn = DateTime.UtcNow
        };

        await _context.UsersBookClubs.AddAsync(membership);
        await _context.SaveChangesAsync();
    }

    public async Task LeaveBookClubAsync(string bookClubId, string userId)
    {
        var membership = await _context.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        if (membership != null)
        {
            membership.IsDeleted = true;
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

        if (!bookClub.Books.Any(b => b.Id == bookId))
        {
            bookClub.Books.Add(book);

            if (isCurrentlyReading)
            {
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

        var book = bookClub.Books.FirstOrDefault(b => b.Id == bookId);
        if (book != null)
        {
            bookClub.Books.Remove(book);
            if (bookClub.CurrentBookId == bookId)
            {
                bookClub.CurrentBookId = null;
            }
            await _context.SaveChangesAsync();
        }
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

        if (!bookClub.Books.Any(b => b.Id == bookId))
        {
            throw new InvalidOperationException("Book is not in this book club");
        }

        bookClub.CurrentBookId = bookId;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookDto>> GetBooksAsync(string bookClubId)
    {
        return await _context.BookClubs
            .Where(bc => bc.Id == bookClubId && !bc.IsDeleted)
            .SelectMany(bc => bc.Books)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                Genres = b.Genres
                    .Where(bg => !bg.IsDeleted)
                    .Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        BooksCount = bg.Genre.Books.Count(b => !b.IsDeleted)
                    }).ToList()
            })
            .ToListAsync();
    }

    public async Task<BookDto?> GetCurrentBookAsync(string bookClubId)
    {
        return await _context.BookClubs
            .Where(bc => bc.Id == bookClubId && !bc.IsDeleted && bc.CurrentBookId != null)
            .Select(bc => new BookDto
            {
                Id = bc.Books.First(b => b.Id == bc.CurrentBookId).Id,
                Title = bc.Books.First(b => b.Id == bc.CurrentBookId).Title,
                Author = bc.Books.First(b => b.Id == bc.CurrentBookId).Author,
                Description = bc.Books.First(b => b.Id == bc.CurrentBookId).Description,
                Genres = bc.Books.First(b => b.Id == bc.CurrentBookId).Genres
                    .Where(bg => !bg.IsDeleted)
                    .Select(bg => new GenreDto
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name,
                        BooksCount = bg.Genre.Books.Count(b => !b.IsDeleted)
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<BookClubMembersDto> GetMembersAsync(string bookClubId, string userId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Users)
                .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            throw new InvalidOperationException("Book club not found");
        }

        return new BookClubMembersDto
        {
            BookClubId = bookClub.Id,
            BookClubName = bookClub.Name,
            IsCurrentUserOwner = bookClub.OwnerId == userId,
            IsCurrentUserAdmin = bookClub.Users.Any(u => u.UserId == userId && u.IsAdmin && !u.IsDeleted),
            Members = bookClub.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new MemberDto
                {
                    UserId = u.UserId,
                    UserName = $"{u.User.FirstName} {u.User.LastName}",
                    Email = u.User.Email!,
                    JoinedOn = u.JoinedOn,
                    IsAdmin = u.IsAdmin,
                    IsOwner = bookClub.OwnerId == u.UserId
                }).ToList()
        };
    }

    public async Task SetAdminStatusAsync(string bookClubId, string userId, string currentUserId, bool isAdmin)
    {
        var bookClub = await _context.BookClubs
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null || bookClub.OwnerId != currentUserId)
        {
            throw new InvalidOperationException("Not authorized to change admin status");
        }

        var membership = await _context.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId && ubc.UserId == userId);

        if (membership == null)
        {
            throw new InvalidOperationException("Member not found");
        }

        membership.IsAdmin = isAdmin;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserAdminAsync(string bookClubId, string userId)
    {
        return await _context.UsersBookClubs
            .AnyAsync(ubc => ubc.BookClubId == bookClubId &&
                            ubc.UserId == userId &&
                            ubc.IsAdmin &&
                            !ubc.IsDeleted);
    }

    public async Task<bool> IsUserOwnerOrAdminAsync(string bookClubId, string userId)
    {
        var bookClub = await _context.BookClubs
            .Include(bc => bc.Users)
            .FirstOrDefaultAsync(bc => bc.Id == bookClubId && !bc.IsDeleted);

        if (bookClub == null)
        {
            return false;
        }

        return bookClub.OwnerId == userId ||
               await IsUserAdminAsync(bookClubId, userId);
    }

    public async Task<IEnumerable<BookClubDto>> GetUserClubsAsync(string userId)
    {
        return await _context.BookClubs
            .Where(bc => !bc.IsDeleted &&
                        bc.Users.Any(u => u.UserId == userId && !u.IsDeleted))
            .Select(bc => new BookClubDto
            {
                Id = bc.Id,
                Name = bc.Name,
                Description = bc.Description,
                OwnerId = bc.OwnerId,
                OwnerName = $"{bc.Owner.FirstName} {bc.Owner.LastName}",
                MembersCount = bc.Users.Count(u => !u.IsDeleted),
                IsUserMember = true,
                IsUserOwner = bc.OwnerId == userId,
                IsUserAdmin = bc.Users.Any(u => u.UserId == userId && u.IsAdmin && !u.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<bool> IsUserMemberAsync(string bookClubId, string userId)
    {
        return await _context.UsersBookClubs
            .AnyAsync(ubc => ubc.BookClubId == bookClubId &&
                            ubc.UserId == userId &&
                            !ubc.IsDeleted);
    }
}