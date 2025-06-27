using Blog.Shared.Dtos;

namespace Blog.Web.ApiClients
{
    public class CommentApiClient(HttpClient http)
    {
        public async Task<List<CommentDto>> GetByArticleAsync(int articleId)
        {
            return await http.GetFromJsonAsync<List<CommentDto>>($"api/comments?articleId={articleId}")
                   ?? [];
        }

        public async Task AddCommentAsync(CommentDto dto)
        {
            var res = await http.PostAsJsonAsync("api/comments", dto);
            res.EnsureSuccessStatusCode();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var res = await http.DeleteAsync($"api/comments/{commentId}");
            res.EnsureSuccessStatusCode();
        }
    }

}
