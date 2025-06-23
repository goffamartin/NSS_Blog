using System.ComponentModel.DataAnnotations;

namespace Blog.ApiService.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }
    }
}
