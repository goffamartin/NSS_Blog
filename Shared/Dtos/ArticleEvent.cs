namespace Blog.Shared.Dtos
{
    public class ArticleEvent
    {
        public string Type { get; set; } = ""; // "Updated" or "Deleted"
        public ArticleSearchDto Article { get; set; } = new();
    }
}
