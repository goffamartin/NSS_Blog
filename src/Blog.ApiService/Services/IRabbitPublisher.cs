using Blog.Shared.Dtos;

namespace Blog.ApiService.Services
{
    public interface IRabbitPublisher
    {
        Task PublishArticleEventAsync(string type, ArticleSearchDto article);
    }
}
