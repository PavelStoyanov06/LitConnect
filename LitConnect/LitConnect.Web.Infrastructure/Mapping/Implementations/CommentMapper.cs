namespace LitConnect.Web.Infrastructure.Mapping.Implementations;

using LitConnect.Services.Models;
using LitConnect.Web.Infrastructure.Mapping.Contracts;
using LitConnect.Web.ViewModels.Comment;

public class CommentMapper : ICommentMapper
{
    public CommentViewModel MapToViewModel(CommentDto dto)
    {
        return new CommentViewModel
        {
            Id = dto.Id,
            Content = dto.Content,
            AuthorName = dto.AuthorName,
            CreatedOn = dto.CreatedOn,
            IsCurrentUserAuthor = dto.IsCurrentUserAuthor
        };
    }

    public IEnumerable<CommentViewModel> MapToViewModels(IEnumerable<CommentDto> dtos)
    {
        return dtos.Select(MapToViewModel);
    }
}