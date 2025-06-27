namespace Blog.Shared.Dtos
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public int AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime Created { get; set; }
    }
}
