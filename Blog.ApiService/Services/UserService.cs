using AutoMapper;
using Blog.ApiService.Data;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services
{
    public class UserService(BlogDbContext _db, IMapper _mapper) : IUserService
    {
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _db.Users
                .Where(u => u.Deleted == null)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return user is null || user.Deleted != null
                ? null
                : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(UserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            user.Created = DateTime.UtcNow;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null || user.Deleted != null)
                return false;

            user.Deleted = DateTime.UtcNow;
            user.Banned = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
