using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var cache = builder.AddRedis("cache");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithEnvironment("ELASTIC_CLIENT_APIVERSIONING", "true") // did not fix :(
    .WithDataVolume();

var elasticservice = builder.AddProject<Projects.Blog_ElasticService>("elasticservice")
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch)
    .WithExternalHttpEndpoints();

var apiService = builder.AddProject<Projects.Blog_ApiService>("apiservice")
    .WithReference(rabbitmq)

builder.AddProject<Projects.Blog_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(elasticservice)
    .WithReference(apiService)
    .WithReference(cache)
    .WaitFor(elasticservice)
    .WaitFor(apiService)
    .WaitFor(cache);
    .WaitFor(elasticservice);


builder.AddProject<Projects.Blog_ElasticWorker>("elasticworker")
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(database)
    .WaitFor(elasticsearch)
    .WaitFor(rabbitmq)
    .WaitFor(database);


builder.Build().Run();
