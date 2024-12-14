using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LitConnect.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly LitConnectDbContext _context;

    public CommentService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateAsync(string content, string discussionId, string authorId)
    {
        var comment = new Comment
        {
            Content = content,
            DiscussionId = discussionId,
            AuthorId = authorId,
            CreatedOn = DateTime.UtcNow
        };

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        return comment.Id;
    }

    public async Task DeleteAsync(string id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            comment.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserAuthorAsync(string commentId, string userId)
    {
        return await _context.Comments
            .AnyAsync(c => c.Id == commentId &&
                          c.AuthorId == userId &&
                          !c.IsDeleted);
    }
}