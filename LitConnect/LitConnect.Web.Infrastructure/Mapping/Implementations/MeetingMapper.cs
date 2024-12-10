namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Meeting;

public class MeetingMapper : IMeetingMapper
{
    public MeetingDetailsViewModel MapToDetailsViewModel(MeetingDto dto)
    {
        return new MeetingDetailsViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            ScheduledDate = dto.ScheduledDate,
            BookClubId = dto.BookClubId,
            BookClubName = dto.BookClubName,
            BookTitle = dto.BookTitle
        };
    }

    public MeetingInListViewModel MapToListViewModel(MeetingDto dto)
    {
        return new MeetingInListViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            ScheduledDate = dto.ScheduledDate,
            BookTitle = dto.BookTitle
        };
    }

    public IEnumerable<MeetingInListViewModel> MapToListViewModels(IEnumerable<MeetingDto> dtos)
    {
        return dtos.Select(MapToListViewModel);
    }
}
