namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.BookClub;

public interface IBookClubMapper
{
    BookClubAllViewModel MapToAllViewModel(BookClubDto dto);

    BookClubDetailsViewModel MapToDetailsViewModel(BookClubDto dto);

    BookClubMembersViewModel MapToMembersViewModel(BookClubDto dto, bool isCurrentUserOwner, bool isCurrentUserAdmin);

    IEnumerable<BookClubAllViewModel> MapToAllViewModels(IEnumerable<BookClubDto> dtos);
}