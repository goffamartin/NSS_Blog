using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.ApiService.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(512)]
        public string Content { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? Deleted { get; set; }


        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public User Author { get; set; }

        [ForeignKey(nameof(Article))]
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
