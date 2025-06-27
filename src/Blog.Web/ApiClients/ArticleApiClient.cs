using Blog.Shared.Dtos;

namespace Blog.Web.ApiClients
{
    public class ArticleApiClient(HttpClient http)
    {
        public async Task<ArticleDto[]> GetAllAsync()
        {
            return await http.GetFromJsonAsync<ArticleDto[]>("api/articles")
                   ?? [];
        }

        public async Task<ArticleDto?> GetByIdAsync(int id)
        {
            return await http.GetFromJsonAsync<ArticleDto>($"api/articles/{id}");
        }

        public async Task CreateAsync(ArticleDto dto)
        {
            var res = await http.PostAsJsonAsync("api/articles", dto);
            res.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, ArticleDto dto)
        {
            var res = await http.PutAsJsonAsync($"api/articles/{id}", dto);
            res.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var res = await http.DeleteAsync($"api/articles/{id}");
            res.EnsureSuccessStatusCode();
        }
    }

}
