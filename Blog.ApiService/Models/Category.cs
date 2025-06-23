using System.ComponentModel.DataAnnotations;

namespace Blog.ApiService.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public ICollection<Article> Articles { get; set; } = [];
    }
}
