namespace Blog.Shared.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int ArticleId { get; set; }
        public DateTime Created { get; set; }
    }
}
