using Blog.Shared.Dtos;

namespace Blog.Web.ApiClients
{
    public class LikeApiClient(HttpClient http)
    {
        public async Task<bool> ToggleLikeAsync(LikeDto dto)
        {
            var response = await http.PostAsJsonAsync("api/likes", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result.Equals("Liked", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<int> GetLikeCountAsync(int articleId)
        {
            return await http.GetFromJsonAsync<int>($"api/likes/{articleId}");
        }
    }

}
