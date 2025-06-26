using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Blog.ApiService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string IdentityProviderExternalId { get; set; }

        public ICollection<Article> Articles { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];

        public bool Banned { get; set; } = false;

        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
