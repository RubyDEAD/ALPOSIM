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
        public ProductStatus Status => Quantity
            switch
            {
                _ when Quantity <= 3 => ProductStatus.Critical,
                _ when Quantity <= MinQuantity => ProductStatus.Low,
                _ when Quantity <= MinQuantity * 2 => ProductStatus.Normal,
                _ => ProductStatus.High

            };
        public int MinQuantity { get; set; } = 0;
        public decimal OriginalPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public String Metric { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
       
        
    }

    public enum ProductStatus
    {
        Critical,
        Low,
        Normal,
        High,
    }
}