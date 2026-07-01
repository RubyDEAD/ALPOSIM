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
        Task<(IEnumerable<Sale> Items, int TotalCount)> GetSalesPageAync(int page, int limit, string? saleCode, bool? payment, DateTime? startDate = null, DateTime? endDate = null);
        
        //Methods
        Task<Sale> CreateSale(Sale sale);
        Task<Sale> UpdateSale(Guid id, Sale sale);
        Task DeleteSale(Guid id);

        //Aggs
        Task<decimal> GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate);
        

    }

    
    
}