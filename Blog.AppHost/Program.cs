
var builder = DistributedApplication.CreateBuilder(args);

// ---- SQL SERVER ----
var sqlServer = builder.AddSqlServer("sql")
    .WithEnvironment("SQL_PASSWORD", "ZUNXYMwXH97zrrhpz-PgDw")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var database = sqlServer.AddDatabase( "BlogDb");
var identityDb = sqlServer.AddDatabase("IdentityDb");

// ---- REDIS ----
var cache = builder.AddRedis("cache")
    .WithEnvironment("CACHE_PASSWORD", "sGWJHgSfuvWF7mM3GJWyp3");

// ---- RABBITMQ ----
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithEnvironment("RABBITMQ_USER", "guest")
    .WithEnvironment("RABBITMQ_PASSWORD", "zXRbXmSWjYWhk28J38YZSB")
    .WithLifetime(ContainerLifetime.Persistent);

// ---- ELASTICSEARCH ----
var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume()
    .WithEnvironment("ELASTIC_CLIENT_APIVERSIONING", "true");

// ---- ELASTIC SERVICE ----
var elasticservice = builder.AddProject<Projects.Blog_ElasticService>("elasticservice")
    .WithReference(elasticsearch)
    .WithReference(database)
    .WaitFor(elasticsearch)
    .WaitFor(database)
    .WithExternalHttpEndpoints();

// ---- ELASTIC WORKER ----
builder.AddProject<Projects.Blog_ElasticWorker>("elasticworker")
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(database)
    .WaitFor(sqlServer)
    .WaitFor(elasticsearch)
    .WaitFor(rabbitmq)
    .WaitFor(database);

// ---- API SERVICE ----
var apiService = builder.AddProject<Projects.Blog_ApiService>("apiservice")
    .WithReference(rabbitmq)
    .WithReference(database)
    .WithReference(identityDb)
    .WithReference(cache)
    .WaitFor(sqlServer)
    .WaitFor(identityDb)
    .WaitFor(database)
    .WaitFor(cache);

// ---- WEB FRONTEND ----
builder.AddProject<Projects.Blog_Web>("webfrontend")
    .WithReference(elasticservice)
    .WithReference(apiService)
    .WithReference(cache)
    .WaitFor(elasticservice)
    .WaitFor(apiService)
    .WaitFor(cache)
    .WithExternalHttpEndpoints();


builder.AddDockerComposeEnvironment("docker-compose");

builder.Build().Run();
