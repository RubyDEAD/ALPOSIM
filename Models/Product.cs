using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alposim.Models
{
    public class Product
    {
        [Key] public Guid Id { get; set; }
        public String ProductCode { get; set; } = string.Empty;
        [Required] public String Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }
        public String ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [NotMapped]
        public string Status => Quantity
            switch
            {
                _ when Quantity <= 3 => ProductStatusConstants.CRITICAL,
                _ when Quantity <= MinQuantity => ProductStatusConstants.LOW,
                _ when Quantity <= MinQuantity * 2 => ProductStatusConstants.NORMAL,
                _ => ProductStatusConstants.HIGH,

            };
        public int MinQuantity { get; set; } = 0;
        public decimal OriginalPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public String Metric { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
    }

    public static class ProductStatusConstants
    {
        public const string CRITICAL = "Critical";
        public const string LOW = "Low";
        public const string NORMAL = "Normal";
        public const string HIGH = "High";
    }
    
    
}