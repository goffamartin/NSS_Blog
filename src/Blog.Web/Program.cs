using Blog.Web.ApiClients;
using Blog.Web.Components;
using Havit.Blazor.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.AddRedisOutputCache("cache");

builder.Services.AddHxServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiServiceUri = new Uri("https+http://apiservice");

builder.Services.AddHttpClient<ArticleApiClient>(client =>
    client.BaseAddress = apiServiceUri);

builder.Services.AddHttpClient<UserApiClient>(client =>
    client.BaseAddress = apiServiceUri);

builder.Services.AddHttpClient<LikeApiClient>(client =>
    client.BaseAddress = apiServiceUri);

builder.Services.AddHttpClient<CommentApiClient>(client =>
    client.BaseAddress = apiServiceUri);

builder.Services.AddHttpClient<ElasticSearchApiClient>(client =>
    client.BaseAddress = new Uri("https://elasticservice"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
