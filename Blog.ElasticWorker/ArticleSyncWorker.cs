using Blog.ApiService.Models;
using Blog.Shared.Dtos;
using Dapper;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.SearchableSnapshots;
using Microsoft.Data.SqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Blog.ElasticWorker;

public class ArticleSyncWorker : BackgroundService
{
    private readonly ILogger<ArticleSyncWorker> _logger;
    private readonly ElasticsearchClient _es;
    private readonly IConnection _rabbit;
    private readonly IServiceScopeFactory _scopeFactory;

    public ArticleSyncWorker(
        ILogger<ArticleSyncWorker> logger,
        ElasticsearchClient es,
        IConnection rabbit,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _es = es;
        _rabbit = rabbit;
        _scopeFactory = scopeFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EnsureIndexExistsAsync();
        await ReindexAllArticlesAsync();
        StartRabbitMqConsumer();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task EnsureIndexExistsAsync()
    {
        _logger.LogInformation("Checking if 'articles' index exists...");
        var existsResponse = await _es.Indices.ExistsAsync("articles");

        if (existsResponse.Exists)
        {
            _logger.LogInformation("'articles' index already exists.");
            return;
        }

        _logger.LogInformation("'articles' index does not exist. Creating...");

        // For NumberOfShards: This determines how your data is split across your cluster.
        // Start with 1. This setting cannot be changed after the index is created without reindexing.
        // Increase this only if you anticipate a very large amount of data (e.g., >50GB).
        const int numberOfShards = 1;

        // For NumberOfReplicas: These are copies of your shards, providing high availability and read scalability.
        // A value of 1 is recommended for production, but requires at least two nodes in your Elasticsearch cluster.
        // For a single-node development cluster, you can set this to 0 to keep the cluster health 'green'.
        // This setting can be changed at any time.
        const int numberOfReplicas = 1;
        var createIndexResponse = await _es.Indices.CreateAsync<ArticleSearchDto>("articles", s => s
        .Settings(se => se
            .NumberOfReplicas(numberOfShards)
            .NumberOfShards(numberOfReplicas)));

        if (createIndexResponse.IsSuccess())
        {
            _logger.LogInformation("'articles' index created successfully.");
        }
        else
        {
            _logger.LogError("Failed to create 'articles' index: {Reason}", createIndexResponse.DebugInformation);
        }
    }

    private async Task ReindexAllArticlesAsync()
    {
        _logger.LogInformation("Re-indexing all articles on startup...");

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SqlConnection>();

        const string articlesQuery = @"
            SELECT a.Id, a.Title, a.Content, a.Created, a.AuthorId, u.Username AS Author, c.Name AS Category
            FROM dbo.Articles a
            JOIN dbo.Users u ON a.AuthorId = u.Id
            JOIN dbo.Categories c ON a.CategoryId = c.Id";

        var articles = (await db.QueryAsync(articlesQuery)).ToList();

        if (!articles.Any())
        {
            _logger.LogInformation("No articles found to reindex.");
            return;
        }

        var articleIds = articles.Select(a => (int)a.Id).ToArray();

        const string tagsQuery = @"
            SELECT at.ArticlesId, t.Name 
            FROM dbo.ArticleTag at
            JOIN dbo.Tags t ON at.TagsId = t.Id
            WHERE at.ArticlesId IN @articleIds AND at.ArticlesId IS NOT NULL AND t.Name IS NOT NULL";

        var tags = await db.QueryAsync(tagsQuery, new { articleIds });

        var tagsByArticleId = tags
            .GroupBy(t => (int)t.ArticlesId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(t => (string)t.Name).ToList());

        var articleSearchDtos = articles.Select(a => new ArticleSearchDto
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            Tags = tagsByArticleId.ContainsKey(a.Id) ? tagsByArticleId[a.Id] : new List<string>(),
            Category = a.Category,
            Author = a.Author,
            AuthorId = a.AuthorId,
            Created = a.Created
        }).ToList();

        var bulkResponse = await _es.BulkAsync(b => b
            .Index("articles")
            .IndexMany(articleSearchDtos, (op, doc) => op.Id(doc.Id))
        );

        if (bulkResponse.IsSuccess())
        {
            _logger.LogInformation("Reindexed {Count} articles.", articleSearchDtos.Count);
        }
        else
        {
            _logger.LogError("Failed to reindex articles: {Reason}", bulkResponse.DebugInformation);
        }
    }

    private void StartRabbitMqConsumer()
    {
        var channel = _rabbit.CreateModel();
        channel.QueueDeclare("articles", true, false, false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += OnEventReceived;
        channel.BasicConsume("articles", true, consumer);
    }

    private async void OnEventReceived(object _, BasicDeliverEventArgs ea)
    {
        try
        {
            var messageJson = Encoding.UTF8.GetString(ea.Body.ToArray());
            var evt = JsonSerializer.Deserialize<ArticleEvent>(messageJson);
            if (evt is null || evt.Article is null)
            {
                _logger.LogWarning("Invalid message received: {Json}", messageJson);
                return;
            }

            switch (evt.Type)
            {
                case "Created":
                case "Updated":
                    var indexResponse = await _es.IndexAsync(evt.Article, idx => idx.Index("articles").Refresh(Refresh.WaitFor));
                    if (indexResponse.IsSuccess())
                    {
                        _logger.LogInformation("Successfully indexed article {Id}", evt.Article.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to index article {Id}. Reason: {Reason}", evt.Article.Id, indexResponse.DebugInformation);
                    }
                    break;
                case "Deleted":
                    var deleteResponse = await _es.DeleteAsync<ArticleSearchDto>(evt.Article.Id, d => d.Index("articles").Refresh(Elastic.Clients.Elasticsearch.Refresh.WaitFor));
                    if (deleteResponse.IsSuccess())
                    {
                        _logger.LogInformation("Successfully deleted article {Id}", evt.Article.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to delete article {Id}. Reason: {Reason}", evt.Article.Id, deleteResponse.DebugInformation);
                    }
                    break;
                default:
                    _logger.LogWarning("Unknown event type: {Type}", evt.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RabbitMQ message");
        }
    }

    public override void Dispose()
    {
        _rabbit.Close();
        base.Dispose();
    }
}