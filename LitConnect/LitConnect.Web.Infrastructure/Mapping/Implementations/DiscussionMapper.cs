namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Comment;
using LitConnect.Web.ViewModels.Discussion;

public class DiscussionMapper : IDiscussionMapper
{
    public DiscussionDetailsViewModel MapToDetailsViewModel(DiscussionDto dto)
    {
        return new DiscussionDetailsViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Content = dto.Content,
            AuthorName = dto.AuthorName,
            BookClubId = dto.BookClubId,
            BookClubName = dto.BookClubName,
            BookTitle = dto.BookTitle,
            CreatedOn = dto.CreatedOn,
            IsCurrentUserAuthor = dto.IsCurrentUserAuthor,
            IsCurrentUserAdmin = dto.IsCurrentUserAdmin,
            IsCurrentUserOwner = dto.IsCurrentUserOwner,
            Comments = dto.Comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                AuthorName = c.AuthorName,
                CreatedOn = c.CreatedOn,
                IsCurrentUserAuthor = c.IsCurrentUserAuthor
            }).ToList(),
            NewComment = new CommentCreateViewModel
            {
                DiscussionId = dto.Id
            }
        };
    }

    public DiscussionInListViewModel MapToListViewModel(DiscussionDto dto)
    {
        return new DiscussionInListViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            AuthorName = dto.AuthorName,
            CreatedOn = dto.CreatedOn,
            BookTitle = dto.BookTitle
        };
    }

    public IEnumerable<DiscussionInListViewModel> MapToListViewModels(IEnumerable<DiscussionDto> dtos)
    {
        return dtos.Select(MapToListViewModel);
    }
}
