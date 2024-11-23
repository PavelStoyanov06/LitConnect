namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Web.ViewModels.Discussion;
using Microsoft.EntityFrameworkCore;

public class DiscussionService : IDiscussionService
{
    private readonly LitConnectDbContext _context;

    public DiscussionService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DiscussionInListViewModel>> GetBookClubDiscussionsAsync(string bookClubId)
    {
        return await _context.Discussions
            .Where(d => d.BookClubId == bookClubId && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedOn)
            .Select(d => new DiscussionInListViewModel
            {
                Id = d.Id,
                Title = d.Title,
                AuthorName = $"{d.Author.FirstName} {d.Author.LastName}",
                CreatedOn = d.CreatedOn,
                BookTitle = d.Book != null ? d.Book.Title : null
            })
            .ToListAsync();
    }

    public async Task<DiscussionDetailsViewModel?> GetDetailsAsync(string id, string userId)
    {
        return await _context.Discussions
            .Where(d => d.Id == id && !d.IsDeleted)
            .Select(d => new DiscussionDetailsViewModel
            {
                Id = d.Id,
                Title = d.Title,
                Content = d.Content,
                AuthorName = $"{d.Author.FirstName} {d.Author.LastName}",
                BookClubId = d.BookClubId,
                BookClubName = d.BookClub.Name,
                BookTitle = d.Book != null ? d.Book.Title : null,
                CreatedOn = d.CreatedOn,
                IsCurrentUserAuthor = d.AuthorId == userId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(DiscussionCreateViewModel model, string authorId)
    {
        var discussion = new Discussion
        {
            Title = model.Title,
            Content = model.Content,
            BookClubId = model.BookClubId,
            AuthorId = authorId,
            BookId = model.BookId,
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

    public async Task<bool> IsUserAuthorAsync(string discussionId, string userId)
    {
        return await _context.Discussions
            .AnyAsync(d => d.Id == discussionId && d.AuthorId == userId);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Discussions
            .AnyAsync(d => d.Id == id && !d.IsDeleted);
    }
}