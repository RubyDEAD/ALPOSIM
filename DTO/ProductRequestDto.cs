using System.ComponentModel.DataAnnotations;

namespace alposim.DTO
{
    public class ProductRequestDto
    {
        [Required]
        [MaxLength(150)]
        public String Name {get; set;} = string.Empty;
        [Url]
        public String ImageUrl {get; set;} = string.Empty;
        [Range (0, int.MaxValue)]
        public int Quantity {get; set;}
        [Range (0.01, double.MaxValue)]
        public decimal Price {get; set;}
        [MaxLength(100)]
        public String Metric {get; set;} = string.Empty;
    }
}