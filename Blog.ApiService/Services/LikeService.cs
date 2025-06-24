using AutoMapper;
using Blog.ApiService.Data;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services
{
    public class LikeService(BlogDbContext db, IMapper mapper) : ILikeService
    {
        public async Task<bool> ToggleLikeAsync(LikeDto dto)
        {
            var existing = await db.Likes
                .FirstOrDefaultAsync(l => l.UserId == dto.UserId 
                                       && l.ArticleId == dto.ArticleId);

            if (existing != null)
            {
                db.Likes.Remove(existing);
            }
            else
            {
                var like = mapper.Map<Like>(dto);
                like.Created = DateTime.UtcNow;
                db.Likes.Add(like);
            }

            await db.SaveChangesAsync();
            return existing == null; // true = added, false = removed
        }

        public async Task<int> GetLikeCountAsync(int articleId)
        {
            return await db.Likes.CountAsync(l => l.ArticleId == articleId);
        }
    }
}
