namespace LitConnect.Web.Infrastructure.Mapping.Contracts;

using LitConnect.Services.Models;
using LitConnect.Web.ViewModels.Comment;

public interface ICommentMapper
{
    CommentViewModel MapToViewModel(CommentDto dto);

    IEnumerable<CommentViewModel> MapToViewModels(IEnumerable<CommentDto> dtos);
}