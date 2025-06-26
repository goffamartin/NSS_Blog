using Blog.Shared.Dtos;

namespace Blog.Web.ApiClients
{
    public class ElasticSearchApiClient(HttpClient http, ILogger<ElasticSearchApiClient> logger)
    {
        public async Task<List<ArticleSearchDto>> SearchAsync(string query)
        {
            try
            {
                logger.LogInformation("Calling /search/articles with query: {Query}", query);
                var response = await http.GetAsync($"/search/articles?q={Uri.EscapeDataString(query)}");

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Failed to search articles. Status code: {StatusCode}", response.StatusCode);
                    return new();
                }

                var articles = await response.Content.ReadFromJsonAsync<List<ArticleSearchDto>>();
                logger.LogInformation("Found {Count} articles for query: {Query}", articles?.Count ?? 0, query);
                return articles ?? new();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while searching articles.");
                return new();
            }
        }

        public async Task<List<string>> SuggestTitlesAsync(string prefix)
        {
            try
            {
                logger.LogInformation("Calling /search/suggest with prefix: {Prefix}", prefix);
                var response = await http.GetAsync($"/search/suggest?prefix={Uri.EscapeDataString(prefix)}");

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Failed to get suggestions. Status code: {StatusCode}", response.StatusCode);
                    return new();
                }

                var titles = await response.Content.ReadFromJsonAsync<List<string>>();
                logger.LogInformation("Received {Count} suggestions.", titles?.Count ?? 0);
                return titles ?? new();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while retrieving suggestions.");
                return new();
            }
        }

        public async Task<List<ArticleSearchDto>> GetAllArticlesAsync()
        {
            try
            {
                logger.LogInformation("Calling /search/articles/all to retrieve all articles.");
                var response = await http.GetAsync("/search/articles/all");

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Failed to get all articles. Status code: {StatusCode}", response.StatusCode);
                    return new();
                }

                var articles = await response.Content.ReadFromJsonAsync<List<ArticleSearchDto>>();
                logger.LogInformation("Received {Count} total articles.", articles?.Count ?? 0);
                return articles ?? new();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while retrieving all articles.");
                return new();
            }
        }
    }

}
