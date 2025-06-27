using Blog.Shared.Dtos;

namespace Blog.ApiService.Services
{
    public interface ICommentService
    {
        Task<CommentDto?> GetByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetByArticleIdAsync(int articleId);
        Task<CommentDto> CreateAsync(CommentDto dto);
        Task<CommentDto> UpdateAsync(CommentDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}
