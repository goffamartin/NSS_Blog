using Elastic.Clients.Elasticsearch;
using Blog.ElasticService.Dtos;
using Elastic.Clients.Elasticsearch.QueryDsl;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddElasticsearchClient("elasticsearch");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapGet("/search/articles", async (string q, ElasticsearchClient es) =>
{
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
        return Results.Problem("Search failed or returned null.");
    }

    return Results.Ok(response.Documents);
})
.WithName("SearchArticles")
.WithTags("Search");

app.MapGet("/search/suggest", async (string prefix, ElasticsearchClient es) =>
{
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

    return Results.Ok(response.Documents.Select(d => d.Title));
})
.WithName("Autocomplete")
.WithTags("Search");


app.Run();
