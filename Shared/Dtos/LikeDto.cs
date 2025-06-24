namespace Blog.Shared.Dtos
{
    public class LikeDto
    {
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }
    }
}
