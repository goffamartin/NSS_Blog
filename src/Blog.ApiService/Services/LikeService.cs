using AutoMapper;
using Blog.ApiService.Data;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services
{
    public class LikeService(BlogDbContext _db, IMapper _mapper) : ILikeService
    {
        public async Task<bool> ToggleLikeAsync(LikeDto dto)
        {
            var existing = await _db.Likes
                .FirstOrDefaultAsync(l => l.UserId == dto.UserId 
                                       && l.ArticleId == dto.ArticleId);

            if (existing != null)
            {
                _db.Likes.Remove(existing);
            }
            else
            {
                var like = _mapper.Map<Like>(dto);
                like.Created = DateTime.UtcNow;
                _db.Likes.Add(like);
            }

            await _db.SaveChangesAsync();
            return existing == null; // true = added, false = removed
        }

        public async Task<int> GetLikeCountAsync(int articleId)
        {
            return await _db.Likes.CountAsync(l => l.ArticleId == articleId);
        }
    }
}
