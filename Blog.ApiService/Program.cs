using Blog.ApiService.Authentication.Data;
using Blog.ApiService.Data;
using Blog.ApiService.Middleware;
using Blog.ApiService.Seeds;
using Blog.ApiService.Services;
using Blog.AuthService.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRabbitMQClient("rabbitmq");

builder.AddSqlServerClient("IdentityDb");
builder.Services.AddDbContext<IdentityDbContext>(opts =>
  opts.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb"), x =>
    x.MigrationsHistoryTable("__EFMigrationsHistory_Identity"))
);

builder.AddSqlServerClient("BlogDb");
builder.Services.AddDbContext<BlogDbContext>(opts =>
  opts.UseSqlServer(builder.Configuration.GetConnectionString("BlogDb"), x =>
    x.MigrationsHistoryTable("__EFMigrationsHistory_Blog"))
);


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILikeService, LikeService>();

builder.Services.AddScoped<IRabbitPublisher, RabbitPublisher>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Blog API V1");
    });
}

app.MapPost("/seed", async (BlogDbContext db, IWebHostEnvironment env) =>
{
    if (!env.IsDevelopment())
        return Results.Forbid();

    await IdentitySeeder.InitializeAsync(app.Services);
    await DataSeeder.InitializeAsync(db);
    return Results.Ok("Seeding complete");
});

app.MapGet("users/me", async (ClaimsPrincipal claims, IdentityDbContext context) =>
{
    string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    return await context.Users.FindAsync(userId);
})
.RequireAuthorization();

app.MapDefaultEndpoints();

app.MapControllers();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<BlogDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();
}

// Interceptor/Middleware for logging requests
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapIdentityApi<User>();

app.Run();
