using AutoMapper;
using Blog.ApiService.Data;
using Blog.Shared.Dtos;
using Blog.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.ApiService.Services;

public class ArticleService(
    BlogDbContext _db,
    IRabbitPublisher _publisher,
    IMapper _mapper) : IArticleService
{

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
        article.Id = default;
        article.Created = DateTime.UtcNow;
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();

        await _publisher.PublishArticleEventAsync("Created", _mapper.Map<ArticleSearchDto>(article));

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

        await _publisher.PublishArticleEventAsync("Updated", _mapper.Map<ArticleSearchDto>(existing));

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null || article.Deleted != null)
            return false;

        article.Deleted = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _publisher.PublishArticleEventAsync("Deleted", _mapper.Map<ArticleSearchDto>(article));

        return true;
    }

    public async Task<IEnumerable<ArticleDto>> GetByAuthorAsync(int authorId)
    {
        var articles = await _db.Articles
            .Where(a => a.AuthorId == authorId && a.Deleted == null)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<IEnumerable<ArticleDto>> SearchAsync(string searchTerm)
    {
        var articles = await _db.Articles
            .Where(a => (a.Title.Contains(searchTerm) || a.Content.Contains(searchTerm)) && a.Deleted == null)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ArticleDto>>(articles);
    }

    public async Task<int> SoftDeleteByAuthorAsync(int authorId)
    {
        var articles = await _db.Articles
            .Where(a => a.AuthorId == authorId && a.Deleted == null)
            .ToListAsync();

        foreach (var a in articles)
            a.Deleted = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return articles.Count;
    }

}
