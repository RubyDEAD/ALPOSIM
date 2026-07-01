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
            var query = _context.Sales.AsQueryable();
            if (!string.IsNullOrEmpty(saleCode))
                query = query.Where(q => q.SaleCode.Contains(saleCode));

            if (payment != null)
                query = query.Where(q => q.OnlinePayment == payment);
            
            if(startDate.HasValue)
                query = query.Where(q => q.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(q => q.CreatedAt <= endDate.Value);

            var allItems = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

            var totalCount = allItems.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / limit);

            var items = allItems
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return (items, totalCount);
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
            if (DateTime.UtcNow < existingSale.CreatedAt.AddMinutes(3))
                throw new InvalidOperationException("Sale can only be edited within 3 minutes of creation.");
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
            // Use a transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == saleId);
                
                if (sale == null) 
                    throw new KeyNotFoundException($"Sale {saleId} not found");
                
                // Check if sale is already completed/cancelled (optional)
                // if (sale.IsCompleted) throw new InvalidOperationException("Cannot modify completed sale");
                
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == saleItem.ProductId);
                
                if (product == null) 
                    throw new KeyNotFoundException($"Product {saleItem.ProductId} not found");
                
                // Validate quantity
                if (saleItem.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");
                    
                if (product.Quantity < saleItem.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for {product.Name}. Available: {product.Quantity}, Requested: {saleItem.Quantity}");
                
                // Check if item already exists in sale
                var existingItem = sale.Items.FirstOrDefault(i => i.ProductId == saleItem.ProductId);
                if (existingItem != null)
                {
                    // Option 1: Update quantity instead
                    var newQuantity = existingItem.Quantity + saleItem.Quantity;
                    if (product.Quantity < newQuantity)
                        throw new InvalidOperationException($"Insufficient stock for {product.Name}. Available: {product.Quantity}, Requested: {newQuantity}");
                    
                    product.Quantity -= saleItem.Quantity;
                    existingItem.Quantity = newQuantity;
                }
                else
                {
                    // Add new item
                    saleItem.Id = Guid.NewGuid(); // Ensure new GUID
                    saleItem.SaleId = saleId;
                    saleItem.CostPrice = product.OriginalPrice;
                    saleItem.UnitPrice = product.SellingPrice;
                    
                    sale.Items.Add(saleItem);
                    product.Quantity -= saleItem.Quantity;
                }
                
                // Recalculate total
                sale.TotalPrice = sale.Items.Sum(i => i.UnitPrice * i.Quantity);
                sale.ModifiedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveItemAsync(Guid saleId, Guid saleItemId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
    
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == saleId);

                if (sale == null) 
                    throw new KeyNotFoundException($"Sale {saleId} not found");
        
                var saleItem = sale.Items.FirstOrDefault(i => i.Id == saleItemId);
                if (saleItem == null) 
                    throw new KeyNotFoundException($"Sale Item {saleItemId} not found");

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == saleItem.ProductId);
        
                if (product == null) 
                    throw new KeyNotFoundException($"Product {saleItem.ProductId} not found");

                // Restore product quantity
                product.Quantity += saleItem.Quantity;
        
                // Remove item
                sale.Items.Remove(saleItem);
        
                // Recalculate total
                sale.TotalPrice = sale.Items.Sum(i => i.UnitPrice * i.Quantity);
                sale.ModifiedAt = DateTime.UtcNow;
        
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateItemQuantityAsync(Guid saleId, Guid saleItemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");
    
            using var transaction = await _context.Database.BeginTransactionAsync();
    
            try
            {
                var sale = await _context.Sales
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == saleId);
        
                if (sale == null) 
                    throw new KeyNotFoundException($"Sale {saleId} not found");
        
                var saleItem = sale.Items.FirstOrDefault(i => i.Id == saleItemId);
                if (saleItem == null) 
                    throw new KeyNotFoundException($"Sale Item {saleItemId} not found");

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == saleItem.ProductId);
        
                if (product == null) 
                    throw new KeyNotFoundException($"Product {saleItem.ProductId} not found");

                var difference = quantity - saleItem.Quantity;
        
                if (difference > 0)
                {
                    // Increasing quantity - check stock
                    if (product.Quantity < difference)
                        throw new InvalidOperationException(
                            $"Insufficient stock for {product.Name}. Available: {product.Quantity}, Additional needed: {difference}");
            
                    product.Quantity -= difference;
                }
                else if (difference < 0)
                {
                    // Decreasing quantity - return to stock
                    product.Quantity += Math.Abs(difference);
                }
        
                saleItem.Quantity = quantity;
                sale.TotalPrice = sale.Items.Sum(i => i.UnitPrice * i.Quantity);
                sale.ModifiedAt = DateTime.UtcNow;
        
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}