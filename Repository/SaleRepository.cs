using alposim.Data;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.EntityFrameworkCore;

namespace alposim.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly LocalDbContext _context;

        public SaleRepository(LocalDbContext context)
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
            return await _context.Sales
                .Include(s => s.Items)
                .ToListAsync();
        }

        public async Task<Sale> GetSaleById(Guid id)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);
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

        public async Task<(IEnumerable<Sale> Items, int TotalCount)> GetSalesPageAync(int page, int limit, string? saleCode, bool? payment, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Sales
                .Include(s => s.Items)
                .AsQueryable();
            if (!string.IsNullOrEmpty(saleCode))
                query = query.Where(q => EF.Functions.ILike(q.SaleCode, $"%{saleCode}%"));

            if (payment != null)
                query = query.Where(q => q.OnlinePayment == payment);
            
            if(startDate.HasValue)
                query = query.Where(q => q.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(q => q.CreatedAt <= endDate.Value);

            var allItems = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            var totalCount = allItems.Count;

            var items = allItems
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return (items, totalCount);
        }

        public async Task<Sale> CreateSale(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in sale.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null) 
                        throw new KeyNotFoundException($"Product Not found {item.ProductId}");

                    if (product.Quantity < item.Quantity)
                        throw new InvalidOperationException("Insufficient Product Stock for " +
                                                            $"product {product.Name}. Available: {product.Quantity}, Requested: {item.Quantity}");

                    item.CostPrice = product.OriginalPrice;
                    item.UnitPrice = product.SellingPrice;
                    item.Name = product.Name;
                    item.TotalPrice = product.SellingPrice * item.Quantity;
                    item.Id = Guid.NewGuid();
                    product.Quantity -= item.Quantity;
                }

                sale.TotalPrice = sale.Items.Sum(i => i.UnitPrice * i.Quantity);
                sale.Id = Guid.NewGuid();
                sale.SaleCode = await GenerateSaleCodeAsync();
                sale.CreatedAt = DateTime.UtcNow;
                sale.ModifiedAt = DateTime.UtcNow;
                
                if (sale.ReceivedCash < sale.TotalPrice)
                {
                    throw new InvalidOperationException(
                        $"Insufficient cash. Total: {sale.TotalPrice}, Received: {sale.ReceivedCash}");
                }

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return sale;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Sale> UpdateSale(Guid id, Sale updatedSale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var existingSale = await _context.Sales
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == id);
                
                if (existingSale == null)
                    throw new KeyNotFoundException($"Sale {id} not found");
                
                if (DateTime.UtcNow > existingSale.CreatedAt.AddMinutes(3))
                    throw new InvalidOperationException("Sale can only be edited within 3 minutes of creation.");
                
                // Restore stock for existing items
                foreach (var existingItem in existingSale.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == existingItem.ProductId);
                    
                    if (product != null)
                    {
                        product.Quantity += existingItem.Quantity;
                    }
                }
                
                // Clear existing items
                existingSale.Items.Clear();
                
                // Process new items
                foreach (var newItem in updatedSale.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == newItem.ProductId);
                    
                    if (product == null)
                        throw new KeyNotFoundException($"Product {newItem.ProductId} not found");
                    
                    if (product.Quantity < newItem.Quantity)
                        throw new InvalidOperationException(
                            $"Insufficient stock for {product.Name}. Available: {product.Quantity}, Requested: {newItem.Quantity}");
                    
                    var saleItem = new SaleItem
                    {
                        Id = Guid.NewGuid(),
                        SaleId = id,
                        ProductId = newItem.ProductId,
                        Quantity = newItem.Quantity,
                        CostPrice = product.OriginalPrice,
                        UnitPrice = product.SellingPrice
                    };
                    
                    product.Quantity -= newItem.Quantity;
                    existingSale.Items.Add(saleItem);
                }
                
                existingSale.TotalPrice = existingSale.Items.Sum(i => i.UnitPrice * i.Quantity);
                existingSale.ReceivedCash = updatedSale.ReceivedCash;
                existingSale.OnlinePayment = updatedSale.OnlinePayment;
                existingSale.ModifiedAt = DateTime.UtcNow;
                
                if (existingSale.OnlinePayment != true && existingSale.ReceivedCash < existingSale.TotalPrice)
                {
                    throw new InvalidOperationException(
                        $"Insufficient cash. Total: {existingSale.TotalPrice}, Received: {existingSale.ReceivedCash}");
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return existingSale;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteSale(Guid id)
        {
            var sale = await _context.Sales.FindAsync(id);
            
            // ✅ FIX: Check null FIRST before using
            if (sale == null) 
                return;
            
            if (DateTime.UtcNow > sale.CreatedAt.AddMinutes(3))
                throw new InvalidOperationException($"Sale Code: {sale.SaleCode} cannot be deleted anymore.");
            
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .SumAsync(s => s.TotalPrice);
        }
    }
}