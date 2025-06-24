using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", username, password)
    .WithManagementPlugin();

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume();

var elasticservice = builder.AddProject<Projects.Blog_ElasticService>("elasticservice")
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch)
    .WithExternalHttpEndpoints();

var apiService = builder.AddProject<Projects.Blog_ApiService>("apiservice")
    .WithReference(rabbitmq);

builder.AddProject<Projects.Blog_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(elasticservice)
    .WaitFor(elasticservice);


builder.AddProject<Projects.Blog_ElasticWorker>("blog-elasticworker")
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch)
    .WithReference(rabbitmq);


builder.Build().Run();
