using Blog.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;

namespace Blog.ApiService.Services
{
    public class TagService(BlogDbContext _db) : ITagService
    {
        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            return await _db.Tags
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}
