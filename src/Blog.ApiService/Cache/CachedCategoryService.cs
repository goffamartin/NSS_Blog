using Blog.ApiService.Services;
using Shared.Dtos;

namespace Blog.ApiService.Cache
{
    public class CachedCategoryService : ICategoryService
    {
        private readonly ICategoryService _inner;
        private readonly ICacheService _cache;

        public CachedCategoryService(ICategoryService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            const string key = "categories:all";
            var cached = await _cache.GetAsync<IEnumerable<CategoryDto>>(key);
            if (cached is not null) return cached;

            var result = await _inner.GetAllAsync();
            await _cache.SetAsync(key, result, TimeSpan.FromHours(1));
            return result;
        }
    }
}
