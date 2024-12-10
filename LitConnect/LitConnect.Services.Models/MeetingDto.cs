namespace LitConnect.Services.Models;

public class MeetingDto
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string BookClubId { get; set; } = null!;

    public string BookClubName { get; set; } = null!;

    public string? BookTitle { get; set; }
}