using Shared.Dtos;

namespace Blog.ApiService.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
    }
}