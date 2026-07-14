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
        public string Status
        {
            get
            {
                if (Quantity == 0) return ProductStatusConstants.CRITICAL;
                if (Quantity <= 3) return ProductStatusConstants.CRITICAL;

                if (MinQuantity > 0)
                {
                    if (Quantity <= MinQuantity) return ProductStatusConstants.LOW;
                    if (Quantity <= MinQuantity * 2) return ProductStatusConstants.NORMAL;
                    return ProductStatusConstants.HIGH;
                }


                if (Quantity <= 5) return ProductStatusConstants.LOW;
                if (Quantity <= 20) return ProductStatusConstants.NORMAL;
                return ProductStatusConstants.HIGH;
            }
        }
        
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