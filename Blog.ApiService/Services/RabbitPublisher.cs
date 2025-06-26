using Blog.Shared.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Blog.ApiService.Services
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitPublisher> _logger;
        private readonly IConnection _messageConnection;
        private readonly IModel _messageChannel;

        public RabbitPublisher(IServiceProvider serviceProvider, ILogger<RabbitPublisher> logger)
        {
            _serviceProvider = serviceProvider;

            _messageConnection = _serviceProvider.GetRequiredService<IConnection>();

            _messageChannel = _messageConnection.CreateModel();
            _logger = logger;

            _messageChannel.QueueDeclare("articles", durable: true, exclusive: false, autoDelete: false);
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

            _messageChannel.BasicPublish(exchange: "",
                                  routingKey: "articles",
                                  basicProperties: null,
                                  body: bytes);

            _logger.LogInformation("Published {Type} event for article {Id}", type, article.Id);
            return Task.CompletedTask;
        }
    }
}
