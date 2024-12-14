using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace LitConnect.Services.Implementations;

public class DiscussionService : IDiscussionService
{
    private readonly LitConnectDbContext _context;

    public DiscussionService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DiscussionDto>> GetBookClubDiscussionsAsync(string bookClubId)
    {
        return await _context.Discussions
            .Where(d => d.BookClubId == bookClubId && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedOn)
            .Select(d => new DiscussionDto
            {
                Id = d.Id,
                Title = d.Title,
                AuthorName = $"{d.Author.FirstName} {d.Author.LastName}",
                CreatedOn = d.CreatedOn,
                BookTitle = d.Book != null ? d.Book.Title : null
            })
            .ToListAsync();
    }

    public async Task<DiscussionDto?> GetDetailsAsync(string id, string userId)
    {
        Console.WriteLine(await _context.Discussions
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted));

        return await _context.Discussions
            .Where(d => d.Id == id && !d.IsDeleted)
            .Select(d => new DiscussionDto
            {
                Id = d.Id,
                Title = d.Title,
                Content = d.Content,
                AuthorName = $"{d.Author.FirstName} {d.Author.LastName}",
                BookClubId = d.BookClubId,
                BookClubName = d.BookClub.Name,
                BookTitle = d.Book != null ? d.Book.Title : null,
                CreatedOn = d.CreatedOn,
                IsCurrentUserAuthor = d.AuthorId == userId,
                IsCurrentUserAdmin = d.BookClub.Users.Any(u => u.UserId == userId && u.IsAdmin && !u.IsDeleted),
                IsCurrentUserOwner = d.BookClub.OwnerId == userId,
                Comments = d.Comments
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.CreatedOn)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        AuthorName = $"{c.Author.FirstName} {c.Author.LastName}",
                        CreatedOn = c.CreatedOn,
                        IsCurrentUserAuthor = c.AuthorId == userId
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(string title, string content, string bookClubId, string? bookId, string authorId)
    {
        var discussion = new Discussion
        {
            Title = title,
            Content = content,
            BookClubId = bookClubId,
            BookId = bookId,
            AuthorId = authorId,
            CreatedOn = DateTime.UtcNow
        };

        await _context.Discussions.AddAsync(discussion);
        await _context.SaveChangesAsync();

        return discussion.Id;
    }

    public async Task DeleteAsync(string id)
    {
        var discussion = await _context.Discussions.FindAsync(id);
        if (discussion != null)
        {
            discussion.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CanUserDeleteAsync(string discussionId, string userId)
    {
        var discussion = await _context.Discussions
            .Include(d => d.BookClub)
                .ThenInclude(bc => bc.Users)
            .FirstOrDefaultAsync(d => d.Id == discussionId && !d.IsDeleted);

        if (discussion == null)
        {
            return false;
        }

        return discussion.AuthorId == userId ||
               discussion.BookClub.Users.Any(u => u.UserId == userId && u.IsAdmin && !u.IsDeleted) ||
               discussion.BookClub.OwnerId == userId;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Discussions
            .AnyAsync(d => d.Id == id && !d.IsDeleted);
    }
}