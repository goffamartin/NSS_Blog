namespace Blog.ApiService.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var query = context.Request.QueryString;
        var time = DateTime.UtcNow;

        _logger.LogInformation("--> {Method} {Path}{Query} at {Time}", method, path, query, time);

        await _next(context);

        _logger.LogInformation("<-- {Method} {Path} responded with {StatusCode}",
            method, path, context.Response.StatusCode);
    }
}
