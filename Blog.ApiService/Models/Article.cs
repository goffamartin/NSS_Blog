using Microsoft.Extensions.Diagnostics.Latency;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Blog.ApiService.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required, MaxLength(512)]
        public string Content { get; set; }

        public bool Disabled { get; set; } = false;

        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public User Author { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Tag> Tags { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
    }
}
