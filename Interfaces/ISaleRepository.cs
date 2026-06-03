using alposim.Models;

namespace alposim.Interfaces
{

    public interface ISaleRepository
    {

        //Queries
        Task<IEnumerable<Sale>> GetSales();
        Task<Sale> GetSaleById(Guid id);
        Task<IEnumerable<Sale>> GetSalesFromDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sale>> GetSalesByPayment(bool payment);
        
        //Methods
        Task<Sale> CreateSale(Sale sale);
        Task<Sale> UpdateSale(Guid id, Sale sale);
        Task DeleteSale(Guid id);

        //Aggs
        Task<decimal> GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate);


        //Item Management
        Task AddItemAsync(Guid saleId, SaleItem saleItem);
        Task RemoveItemAsync(Guid saleId, Guid saleItemId);
        Task UpdateItemQuantityAsync(Guid saleId, Guid saleItemId, int quantity);

    }

    
    
}