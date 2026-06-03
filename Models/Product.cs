using System.ComponentModel.DataAnnotations;

namespace alposim.Models
{
    public class Product
    {
        [Key]
        public Guid Id {get; set;}
        public String ProductCode {get; set;} = string.Empty;
        [Required]
        public String Name {get; set;} = string.Empty;
        public String ImageUrl {get; set;} = string.Empty;
        public int Quantity {get; set;}
        
        public decimal OriginalPrice {get; set;}
        public decimal SellingPrice {get; set;}
        public String Metric {get; set;} = string.Empty;
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}