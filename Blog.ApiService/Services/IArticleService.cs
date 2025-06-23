using Blog.ApiService.Dtos;

namespace Blog.ApiService.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetAllAsync();
        Task<ArticleDto?> GetByIdAsync(int id);
        Task<ArticleDto> CreateAsync(ArticleDto dto);
        Task<bool> UpdateAsync(int id, ArticleDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<ArticleDto>> GetByAuthorAsync(int authorId);
        Task<IEnumerable<ArticleDto>> SearchAsync(string searchTerm);
        Task<int> SoftDeleteByAuthorAsync(int authorId);
    }
}
