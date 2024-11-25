namespace LitConnect.Web.ViewModels.Meeting;

public class MeetingInListViewModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime ScheduledDate { get; set; }

    public string? BookTitle { get; set; }

    public bool IsUpcoming => ScheduledDate > DateTime.UtcNow;
}