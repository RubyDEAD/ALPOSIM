using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alposim.Models
{
    public class Sale
    {
        [Key] 
        public Guid Id { get; set; }
        public String SaleCode {get; set;} = string.Empty;
        public ICollection<SaleItem> Items {get; set;} = new List<SaleItem>();
        public decimal TotalPrice {get; set;}
        public decimal ReceivedCash {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public DateTime ModifiedAt {get; set;} = DateTime.UtcNow;
        public bool? OnlinePayment {get; set;}
        [NotMapped]
        public decimal Change => ReceivedCash - TotalPrice;
    }

    public class SaleItem
    {
        [Key]
        public Guid Id {get; set;}
        public Guid SaleId {get; set;}
        public Guid ProductId {get; set;}
        public int Quantity {get; set;}
        public decimal CostPrice { get; set; }
        public decimal UnitPrice {get; set;} //Equivalent to SellingPrice
        public decimal TotalPrice {get; set;}
    }
}