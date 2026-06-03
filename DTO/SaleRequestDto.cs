using alposim.Models;
namespace alposim.DTO;

public class SaleRequestDto
{
    public ICollection<SaleItem> Items {get; set;} = new List<SaleItem>();
    public decimal TotalPrice {get; set;}
    public decimal ReceivedCash {get; set;}
    public bool? OnlinePayment {get; set;}
    
}