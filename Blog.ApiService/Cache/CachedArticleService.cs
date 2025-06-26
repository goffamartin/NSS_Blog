using Blog.ApiService.Services;
using Blog.Shared.Dtos;

namespace Blog.ApiService.Cache
{
    public class CachedArticleService(IArticleService inner, ICacheService cache) : IArticleService
    {
        public async Task<ArticleDto?> GetByIdAsync(int id)
        {
            string key = $"article:{id}";
            var cached = await cache.GetAsync<ArticleDto>(key);
            if (cached is not null) return cached;

            var article = await inner.GetByIdAsync(id);
            if (article is not null)
                await cache.SetAsync(key, article, TimeSpan.FromMinutes(10));

            return article;
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync() => await inner.GetAllAsync();
        public async Task<ArticleDto> CreateAsync(ArticleDto dto) => await inner.CreateAsync(dto);
        public async Task<bool> UpdateAsync(int id, ArticleDto dto)
        {
            var success = await inner.UpdateAsync(id, dto);
            if (success) await cache.RemoveAsync($"article:{id}");
            return success;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var success = await inner.SoftDeleteAsync(id);
            if (success) await cache.RemoveAsync($"article:{id}");
            return success;
        }

        public async Task<IEnumerable<ArticleDto>> GetByAuthorAsync(int authorId)
        {
            string key = $"articles:author:{authorId}";
            var cached = await cache.GetAsync<IEnumerable<ArticleDto>>(key);
            if (cached is not null) return cached;

            var articles = await inner.GetByAuthorAsync(authorId);
            if (articles is not null)
                await cache.SetAsync(key, articles, TimeSpan.FromMinutes(10));

            return articles;
        }

        public async Task<IEnumerable<ArticleDto>> SearchAsync(string searchTerm)
        {
            string key = $"articles:search:{searchTerm}";
            var cached = await cache.GetAsync<IEnumerable<ArticleDto>>(key);
            if (cached is not null) return cached;

            var articles = await inner.SearchAsync(searchTerm);
            if (articles is not null)
                await cache.SetAsync(key, articles, TimeSpan.FromMinutes(5));

            return articles;
        }

        public async Task<int> SoftDeleteByAuthorAsync(int authorId)
        {
            var count = await inner.SoftDeleteByAuthorAsync(authorId);
            if (count > 0)
                await cache.RemoveAsync($"articles:author:{authorId}");
            return count;
        }
    }

}
