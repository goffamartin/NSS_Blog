using Blog.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;

namespace Blog.ApiService.Services
{
    public class CategoryService(BlogDbContext _db) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _db.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}