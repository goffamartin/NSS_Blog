using Blog.Shared.Dtos;
using Elastic.Clients.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddElasticsearchClient("elasticsearch");

builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Blog Elasticsearch API V1");
    });
}

app.MapGet("/search/articles", async (string q, ElasticsearchClient es, ILogger<Program> logger) =>
{
    logger.LogInformation("Searching articles for query: {Query}", q);
    var response = await es.SearchAsync<ArticleSearchDto>(s => s
        .Index("articles")
        .Query(query => query
            .MultiMatch(multi => multi
                .Fields(new[]
                {
                    "title",
                    "content",
                    "tags",
                    "category",
                    "author"
                })
                .Query(q)
                .Fuzziness(new Fuzziness("AUTO") ) // Optional: improves tolerance to typos
            )
        )
    );

    if (!response.IsValidResponse || response.Documents is null)
    {
        logger.LogError("Search failed for query: {Query}. Invalid response from Elasticsearch.", q);
        return Results.Problem("Search failed or returned null.");
    }

    logger.LogInformation("Found {Count} articles for query: {Query}", response.Documents.Count, q);
    return Results.Ok(response.Documents);
})
.WithName("SearchArticles")
.WithTags("Search");

app.MapGet("/search/suggest", async (string prefix, ElasticsearchClient es, ILogger<Program> logger) =>
{
    logger.LogInformation("Suggesting titles for prefix: {Prefix}", prefix);
    var response = await es.SearchAsync<ArticleSearchDto>(s => s
        .Index("articles")
        .Size(5)
        .Query(q => q
            .MatchPhrasePrefix(m => m
                .Field("title")
                .Query(prefix)
            )
        )
    );

    if (!response.IsValidResponse || response.Documents is null)
    {
        logger.LogError("Suggestion search failed for prefix: {Prefix}. Invalid response from Elasticsearch.", prefix);
        return Results.Problem("Search failed or returned null.");
    }

    var titles = response.Documents.Select(d => d.Title).ToList();
    logger.LogInformation("Found {Count} suggestions for prefix: {Prefix}", titles.Count, prefix);
    return Results.Ok(titles);
})
.WithName("Autocomplete")
.WithTags("Search");

app.MapGet("/search/articles/all", async (ElasticsearchClient es, ILogger<Program> logger) =>
{
    logger.LogInformation("Getting all articles");
    var response = await es.SearchAsync<ArticleSearchDto>(s => s
        .Index("articles")
        .Size(1000) // Limit the number of results
    );

    if (!response.IsValidResponse || response.Documents is null)
    {
        logger.LogError("Failed to get all articles. Invalid response from Elasticsearch.");
        return Results.Problem("Search failed or returned null.");
    }

    logger.LogInformation("Found {Count} total articles.", response.Documents.Count);
    return Results.Ok(response.Documents);
})
.WithName("GetAllArticles")
.WithTags("Search");

app.MapDefaultEndpoints();

app.Run();
