namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.Discussion;

public interface IDiscussionMapper
{
    DiscussionDetailsViewModel MapToDetailsViewModel(DiscussionDto dto);

    DiscussionInListViewModel MapToListViewModel(DiscussionDto dto);

    IEnumerable<DiscussionInListViewModel> MapToListViewModels(IEnumerable<DiscussionDto> dtos);
}