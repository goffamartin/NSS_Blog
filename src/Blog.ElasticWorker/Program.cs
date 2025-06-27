using Blog.ElasticWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddRabbitMQClient("rabbitmq");

builder.AddElasticsearchClient("elasticsearch");

builder.AddSqlServerClient("BlogDb");

builder.Services.AddHostedService<ArticleSyncWorker>();

var host = builder.Build();
host.Run();
