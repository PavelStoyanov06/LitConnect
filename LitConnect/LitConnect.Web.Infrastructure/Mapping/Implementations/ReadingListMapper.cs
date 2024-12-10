namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.ReadingList;

public class ReadingListMapper : IReadingListMapper
{
    public ReadingListViewModel MapToViewModel(ReadingListDto dto)
    {
        return new ReadingListViewModel
        {
            Id = dto.Id,
            UserId = dto.UserId,
            UserName = dto.UserName,
            Books = dto.Books.Select(MapToBookViewModel).ToList()
        };
    }

    public ReadingListBookViewModel MapToBookViewModel(ReadingListBookDto dto)
    {
        return new ReadingListBookViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            Status = (ViewModels.ReadingList.ReadingStatus)dto.Status,
            Genres = dto.Genres
        };
    }
}