using Blog.ApiService.Dtos;

namespace Blog.ApiService.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetByArticleIdAsync(int articleId);
        Task<CommentDto> CreateAsync(CommentDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}
