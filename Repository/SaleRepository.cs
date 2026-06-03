using alposim.Data;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.EntityFrameworkCore;

namespace alposim.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _context;

        public SaleRepository(AppDbContext context)
        {
            _context = context;
        }

        private async Task<string> GenerateSaleCodeAsync()
        {
            var yearPart = DateTime.UtcNow.ToString("yy");
            var lastSale = await _context.Sales
                .Where(s => !string.IsNullOrEmpty(s.SaleCode) && s.SaleCode.StartsWith($"SALE-{yearPart}-"))
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            int lastCodeNumber = 0;
            if (lastSale != null)
            {
                var codeParts = lastSale.SaleCode.Split('-');
                if (codeParts.Length == 3 && int.TryParse(codeParts[2], out int parsedNumber))
                {
                    lastCodeNumber = parsedNumber;
                }
            }

            return $"SALE-{yearPart}-{lastCodeNumber + 1:D2}";
        }
        
        public async Task<IEnumerable<Sale>> GetSales()
        {
            return await _context.Sales.ToListAsync();
        }

        public async Task<Sale> GetSaleById(Guid id)
        {
            return await _context.Sales.FindAsync(id);
        }
        
        public async Task<IEnumerable<Sale>> GetSalesFromDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByPayment(bool payment)
        {
            return await _context.Sales
                .Where(s => s.OnlinePayment == payment)
                .ToListAsync();
        }

        public async Task<Sale> CreateSale(Sale sale)
        {
            sale.SaleCode = await GenerateSaleCodeAsync();
            sale.CreatedAt = DateTime.UtcNow;
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return sale;

        }

        public async Task<Sale> UpdateSale(Guid id, Sale sale)
        {
            var existingSale = await _context.Sales.FindAsync(id);
            if (existingSale == null) return null;

            existingSale.TotalPrice = sale.TotalPrice;
            existingSale.OnlinePayment = sale.OnlinePayment;
            existingSale.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingSale;
            
        }

        public async Task DeleteSale(Guid id)
        {
            var sale =  await _context.Sales.FindAsync(id);
            if (sale == null) return ;
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .SumAsync(s => s.TotalPrice);
        }

        public async Task AddItemAsync(Guid saleId, SaleItem saleItem)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId);
        
            if (sale == null) throw new KeyNotFoundException($"Sale {saleId} not found");

            saleItem.SaleId = saleId;  
            sale.Items.Add(saleItem);
            sale.TotalPrice = sale.Items.Sum(i => i.TotalPrice); 

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(Guid saleId, Guid saleItemId)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) throw new KeyNotFoundException($"Sale {saleId} not found");
            var saleItem = sale.Items.FirstOrDefault(i => i.Id == saleItemId);
            if (saleItem == null) throw new KeyNotFoundException($"Sale Item {saleItemId} not found");
            sale.Items.Remove(saleItem);
            sale.TotalPrice = sale.Items.Sum(i => i.TotalPrice);
            
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemQuantityAsync(Guid saleId, Guid saleItemId, int quantity)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId);
            
            
            if (sale == null) throw new KeyNotFoundException($"Sale {saleId} not found");
            var saleItem = sale.Items.FirstOrDefault(i => i.Id == saleId);
            if (saleItem == null) throw new KeyNotFoundException($"Sale Item {saleId} not found");
            saleItem.Quantity = quantity;
            sale.TotalPrice = sale.Items.Sum(i => i.TotalPrice);
            
            await _context.SaveChangesAsync();
        }
    }
}