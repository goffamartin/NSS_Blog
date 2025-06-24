using AutoMapper;
using Blog.ApiService.Data;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services
{
    public class CommentService(BlogDbContext db, IMapper mapper) : ICommentService
    {
        public async Task<IEnumerable<CommentDto>> GetByArticleIdAsync(int articleId)
        {
            var comments = await db.Comments
                .Where(c => c.ArticleId == articleId && c.Deleted == null)
                .OrderByDescending(c => c.Created)
                .ToListAsync();

            return mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<CommentDto> CreateAsync(CommentDto dto)
        {
            var comment = mapper.Map<Comment>(dto);
            comment.Created = DateTime.UtcNow;
            db.Comments.Add(comment);
            await db.SaveChangesAsync();
            return mapper.Map<CommentDto>(comment);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var comment = await db.Comments.FindAsync(id);
            if (comment is null || comment.Deleted != null)
                return false;

            comment.Deleted = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return true;
        }
    }

}
