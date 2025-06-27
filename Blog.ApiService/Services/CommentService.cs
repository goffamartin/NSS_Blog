using AutoMapper;
using Blog.ApiService.Data;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services
{
    public class CommentService(BlogDbContext _db, IMapper _mapper) : ICommentService
    {
        public async Task<CommentDto?> GetByIdAsync(int id)
        {
            var comments = await _db.Comments.FindAsync(id);
            return comments is null || comments.Deleted != null
                ? null
                : _mapper.Map<CommentDto>(comments);
        }

        public async Task<IEnumerable<CommentDto>> GetByArticleIdAsync(int articleId)
        {
            var comments = await _db.Comments
                .Where(c => c.ArticleId == articleId && c.Deleted == null)
                .OrderByDescending(c => c.Created)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<CommentDto> CreateAsync(CommentDto dto)
        {
            var comment = _mapper.Map<Comment>(dto);
            comment.Created = DateTime.UtcNow;
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment is null || comment.Deleted != null)
                return false;

            comment.Deleted = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<CommentDto> UpdateAsync(CommentDto dto)
        {
            var existing = await _db.Comments.FindAsync(dto.Id);
            if (existing == null || existing.Deleted != null)
            {
                throw new KeyNotFoundException($"Comment with ID {dto.Id} not found or already deleted.");
            }
            existing.Content = dto.Content;
            existing.Created = DateTime.UtcNow;
            _db.Comments.Update(existing);
            await _db.SaveChangesAsync(); // Blocking call, consider using async all the way up
            return _mapper.Map<CommentDto>(existing);
        }
    }

}
