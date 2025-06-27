using Blog.ApiService.Services;
using Shared.Dtos;

namespace Blog.ApiService.Cache
{
    public class CachedTagService : ITagService
    {
        private readonly ITagService _inner;
        private readonly ICacheService _cache;

        public CachedTagService(ITagService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            const string key = "tags:all";
            var cached = await _cache.GetAsync<IEnumerable<TagDto>>(key);
            if (cached is not null) return cached;

            var result = await _inner.GetAllAsync();
            await _cache.SetAsync(key, result, TimeSpan.FromHours(1));
            return result;
        }
    }

}
