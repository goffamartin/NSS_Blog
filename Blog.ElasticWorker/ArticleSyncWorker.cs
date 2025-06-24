using Blog.Shared.Dtos;
using Elastic.Clients.Elasticsearch;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Blog.ElasticWorker;

public class ArticleSyncWorker : BackgroundService
{
    private readonly ILogger<ArticleSyncWorker> _logger;
    private readonly ServiceProvider _serviceProvider;
    private readonly ElasticsearchClient _es;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public ArticleSyncWorker(ILogger<ArticleSyncWorker> logger, ElasticsearchClient esClient)
    {
        _logger = logger;
        _es = esClient;

        _connection = _serviceProvider.GetRequiredService<IConnection>();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("articles", durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<ArticleEvent>(json);

            if (message.Type is "Created" or "Updated")
            {
                var result = await _es.IndexAsync(message.Article, i => i.Index("articles"));
                _logger.LogInformation("{Type} article {Id} in Elasticsearch", message.Type, message.Article.Id);
            }
            else if (message.Type == "Deleted")
            {
                await _es.DeleteAsync<ArticleSearchDto>(message.Article.Id.ToString(), d => d.Index("articles"));
                _logger.LogInformation("Deleted article {Id} from Elasticsearch", message.Article.Id);
            }
        };

        _channel.BasicConsume("articles", autoAck: true, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
