namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.Meeting;

public interface IMeetingMapper
{
    MeetingDetailsViewModel MapToDetailsViewModel(MeetingDto dto);

    MeetingInListViewModel MapToListViewModel(MeetingDto dto);

    IEnumerable<MeetingInListViewModel> MapToListViewModels(IEnumerable<MeetingDto> dtos);
}