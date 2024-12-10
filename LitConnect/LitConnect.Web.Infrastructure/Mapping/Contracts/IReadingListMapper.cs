namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.ReadingList;

public interface IReadingListMapper
{
    ReadingListViewModel MapToViewModel(ReadingListDto dto);

    ReadingListBookViewModel MapToBookViewModel(ReadingListBookDto dto);
}