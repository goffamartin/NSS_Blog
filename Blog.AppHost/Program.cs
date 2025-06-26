var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql")
                       .WithLifetime(ContainerLifetime.Persistent)    // Keep between runs
                       .WithDataVolume();

var database = sqlServer.AddDatabase("BlogDb");

var cache = builder.AddRedis("cache");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithEnvironment("ELASTIC_CLIENT_APIVERSIONING", "true") // did not fix :(
    .WithDataVolume();

var elasticservice = builder.AddProject<Projects.Blog_ElasticService>("elasticservice")
    .WithReference(elasticsearch)
    .WithReference(database)
    .WaitFor(elasticsearch)
    .WaitFor(database)
    .WithExternalHttpEndpoints();

var apiService = builder.AddProject<Projects.Blog_ApiService>("apiservice")
    .WithReference(rabbitmq)
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.Blog_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(elasticservice)
    .WithReference(apiService)
    .WithReference(cache)
    .WaitFor(elasticservice)
    .WaitFor(apiService)
    .WaitFor(cache);


builder.AddProject<Projects.Blog_ElasticWorker>("elasticworker")
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(database)
    .WaitFor(elasticsearch)
    .WaitFor(rabbitmq)
    .WaitFor(database);


builder.Build().Run();
