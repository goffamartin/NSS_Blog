using System.ComponentModel.DataAnnotations;

namespace Blog.ApiService.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(32)]
        public string Name { get; set; }

        public ICollection<Article> Articles { get; set; } = [];
    }
}
