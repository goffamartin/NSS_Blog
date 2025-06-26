namespace Blog.Shared.Dtos
{
    public class ArticleSearchDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public string Category { get; set; } = "";
        public string Author { get; set; } = "";
        public int AuthorId { get; set; }
        public DateTime Created { get; set; }
    }
}
