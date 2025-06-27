using Blog.Shared.Dtos;

namespace Blog.ApiService.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetByExternalIdAsync(string externalId);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(UserDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}
