using System.ComponentModel.DataAnnotations;

namespace alposim.Models
{

    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; } = string.Empty;
    }
}