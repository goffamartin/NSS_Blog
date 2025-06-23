using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithDataVolume();

var elasticservice = builder.AddProject<Projects.Blog_ElasticService>("elasticservice")
    .WithReference(elasticsearch)
    .WaitFor(elasticsearch)
    .WithExternalHttpEndpoints();

var apiService = builder.AddProject<Projects.Blog_ApiService>("apiservice");

builder.AddProject<Projects.Blog_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(elasticservice)
    .WaitFor(elasticservice);


builder.Build().Run();
