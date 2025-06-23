using AutoMapper;
using Blog.ApiService.Data;
using Blog.ApiService.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services;

public class ArticleService : IArticleService
{
    private readonly BlogDbContext _db;
    private readonly IMapper _mapper;

    public ArticleService(BlogDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ArticleDto>> GetAllAsync()
    {
        var articles = await _db.Articles
            .Where(a => !a.Deleted.HasValue)
            .OrderByDescending(a => a.Created)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<ArticleDto?> GetByIdAsync(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        return article is null || article.Deleted != null
            ? null
            : _mapper.Map<ArticleDto>(article);
    }

    public async Task<ArticleDto> CreateAsync(ArticleDto dto)
    {
        var article = _mapper.Map<Article>(dto);
        article.Created = DateTime.UtcNow;
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
        return _mapper.Map<ArticleDto>(article);
    }

    public async Task<bool> UpdateAsync(int id, ArticleDto dto)
    {
        var existing = await _db.Articles.FindAsync(id);
        if (existing == null || existing.Deleted != null)
            return false;

        existing.Title = dto.Title;
        existing.Content = dto.Content;
        existing.CategoryId = dto.CategoryId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null || article.Deleted != null)
            return false;

        article.Deleted = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
