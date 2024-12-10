namespace LitConnect.Services.Implementations;

using LitConnect.Data;
using LitConnect.Data.Models;
using LitConnect.Services.Contracts;
using LitConnect.Services.Models;
using Microsoft.EntityFrameworkCore;

public class MeetingService : IMeetingService
{
    private readonly LitConnectDbContext _context;

    public MeetingService(LitConnectDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MeetingDto>> GetBookClubMeetingsAsync(string bookClubId)
    {
        return await _context.Meetings
            .Where(m => m.BookClubId == bookClubId && !m.IsDeleted)
            .OrderByDescending(m => m.ScheduledDate)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ScheduledDate = m.ScheduledDate,
                BookClubId = m.BookClubId,
                BookClubName = m.BookClub.Name,
                BookTitle = m.Book != null ? m.Book.Title : null
            })
            .ToListAsync();
    }

    public async Task<MeetingDto?> GetByIdAsync(string id)
    {
        return await _context.Meetings
            .Where(m => m.Id == id && !m.IsDeleted)
            .Select(m => new MeetingDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ScheduledDate = m.ScheduledDate,
                BookClubId = m.BookClubId,
                BookClubName = m.BookClub.Name,
                BookTitle = m.Book != null ? m.Book.Title : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> CreateAsync(string title, string? description, DateTime scheduledDate, string bookClubId, string? bookId)
    {
        var meeting = new Meeting
        {
            Title = title,
            Description = description,
            ScheduledDate = scheduledDate,
            BookClubId = bookClubId,
            BookId = bookId
        };

        await _context.Meetings.AddAsync(meeting);
        await _context.SaveChangesAsync();

        return meeting.Id;
    }

    public async Task DeleteAsync(string id)
    {
        var meeting = await _context.Meetings.FindAsync(id);
        if (meeting != null)
        {
            meeting.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Meetings
            .AnyAsync(m => m.Id == id && !m.IsDeleted);
    }
}