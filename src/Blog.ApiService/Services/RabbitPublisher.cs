using Blog.Shared.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Blog.ApiService.Services
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly ILogger<RabbitPublisher> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitPublisher(IConnection connection, ILogger<RabbitPublisher> logger)
        {

            _connection = connection;
            _channel = _connection.CreateModel();
            _logger = logger;

            _channel.QueueDeclare("articles", durable: true, exclusive: false, autoDelete: false);
        }

        public Task PublishArticleEventAsync(string type, ArticleSearchDto article)
        {
            var payload = new
            {
                Type = type,
                Article = article
            };

            var json = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "articles",
                                  basicProperties: null,
                                  body: bytes);

            _logger.LogInformation("Published {Type} event for article {Id}", type, article.Id);
            return Task.CompletedTask;
        }
    }
}
