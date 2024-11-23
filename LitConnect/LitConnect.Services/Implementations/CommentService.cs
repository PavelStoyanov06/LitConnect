using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Web.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;

public class CommentService : ICommentService
{
    private readonly LitConnectDbContext _context;

    public CommentService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateAsync(CommentCreateViewModel model, string authorId)
    {
        var comment = new Comment
        {
            Content = model.Content,
            AuthorId = authorId,
            DiscussionId = model.DiscussionId,
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
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserAuthorAsync(string commentId, string userId)
    {
        return await _context.Comments
            .AnyAsync(c => c.Id == commentId && c.AuthorId == userId);
    }
}