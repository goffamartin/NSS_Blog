using Shared.Dtos;

namespace Blog.ApiService.Services
{

    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();
    }

}
