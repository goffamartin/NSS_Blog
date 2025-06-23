using AutoMapper;
using Blog.ApiService.Data;
using Blog.ApiService.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext _db;
        private readonly IMapper _mapper;

        public CommentService(BlogDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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
    }

}
