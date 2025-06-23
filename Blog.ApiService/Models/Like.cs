using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.ApiService.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Article))]
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
