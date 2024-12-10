namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.BookClub;
using LitConnect.Web.ViewModels.Discussion;
using LitConnect.Web.ViewModels.Meeting;

public class BookClubMapper : IBookClubMapper
{
    public BookClubAllViewModel MapToAllViewModel(BookClubDto dto)
    {
        return new BookClubAllViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            MembersCount = dto.MembersCount,
            IsUserMember = dto.IsUserMember
        };
    }

    public BookClubDetailsViewModel MapToDetailsViewModel(BookClubDto dto)
    {
        return new BookClubDetailsViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = dto.OwnerId,
            OwnerName = dto.OwnerName,
            MembersCount = dto.MembersCount,
            IsUserMember = dto.IsUserMember,
            IsUserOwner = dto.IsUserOwner,
            IsUserAdmin = dto.IsUserAdmin,
            CurrentBook = dto.CurrentBook != null ? MapToBookViewModel(dto.CurrentBook) : null,
            Books = dto.Books.Select(MapToBookViewModel).ToList(),
            Discussions = dto.Discussions.Select(d => new DiscussionInListViewModel
            {
                Id = d.Id,
                Title = d.Title,
                AuthorName = d.AuthorName,
                CreatedOn = d.CreatedOn,
                BookTitle = d.BookTitle
            }).ToList(),
            Meetings = dto.Meetings.Select(m => new MeetingInListViewModel
            {
                Id = m.Id,
                Title = m.Title,
                ScheduledDate = m.ScheduledDate,
                BookTitle = m.BookTitle
            }).ToList()
        };
    }

    public BookClubMembersViewModel MapToMembersViewModel(BookClubDto dto, bool isCurrentUserOwner, bool isCurrentUserAdmin)
    {
        return new BookClubMembersViewModel
        {
            BookClubId = dto.Id,
            BookClubName = dto.Name,
            IsCurrentUserOwner = isCurrentUserOwner,
            IsCurrentUserAdmin = isCurrentUserAdmin,
            Members = dto.Members.Select(m => new BookClubMemberViewModel
            {
                UserId = m.UserId,
                UserName = m.UserName,
                Email = m.Email,
                JoinedOn = m.JoinedOn,
                IsAdmin = m.IsAdmin,
                IsOwner = m.IsOwner
            }).ToList()
        };
    }

    public IEnumerable<BookClubAllViewModel> MapToAllViewModels(IEnumerable<BookClubDto> dtos)
    {
        return dtos.Select(MapToAllViewModel);
    }

    private BookClubBookViewModel MapToBookViewModel(BookDto dto)
    {
        return new BookClubBookViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            Genres = dto.Genres.Select(g => g.Name)
        };
    }
}
