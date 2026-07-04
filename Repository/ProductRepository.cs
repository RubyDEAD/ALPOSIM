using alposim.Models;
using alposim.Interfaces;
using Microsoft.EntityFrameworkCore;
using alposim.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace alposim.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly LocalDbContext _context;
        public ProductRepository(LocalDbContext context)
        {
            _context = context;
        }
        private async Task<string> GenerateProductCodeAsync()
        {
            var yearPart = DateTime.UtcNow.ToString("yy");
            var lastProduct = await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.ProductCode) && p.ProductCode.StartsWith($"PRD-{yearPart}-"))
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            int lastCodeNumber = 0;
            if (lastProduct != null)
            {
                var codeParts = lastProduct.ProductCode.Split('-');
                if (codeParts.Length == 3 && int.TryParse(codeParts[2], out int parsedNumber))
                {
                    lastCodeNumber = parsedNumber;
                }
            }

            return $"PRD-{yearPart}-{lastCodeNumber + 1:D2}";
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }
        
        public async Task<Product> GetProductByNameAsync(string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
            if (product == null) return null;
            return product;
        }
        
        public async Task<Product> CreateProductAsync(Product product)
        {
            var productCode = await GenerateProductCodeAsync();
            product.ProductCode = productCode;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        
        public async Task<Product?> UpdateProductAsync(Guid id, Product product, string changedBy)
        {
            var existing = await _context.Products.FindAsync(id); 
            if (existing == null) return null;

            var histories = new List<ProductHistory>();

            if (existing.SellingPrice != product.SellingPrice)
                histories.Add(new ProductHistory
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    FieldChanged = "SellingPrice",
                    OldValue = existing.SellingPrice.ToString(),
                    NewValue = product.SellingPrice.ToString(),
                    ProductCode = product.ProductCode,
                    Action = "Updated",
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                });

            if (existing.Quantity != product.Quantity)
                histories.Add(new ProductHistory
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    FieldChanged = "Quantity",
                    OldValue = existing.Quantity.ToString(),
                    NewValue = product.Quantity.ToString(),
                    Action = existing.Quantity < product.Quantity ? "Restocked" : "Adjusted",
                    ProductCode = existing.ProductCode,
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                });

            if (existing.OriginalPrice != product.OriginalPrice)
                histories.Add(new ProductHistory
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    FieldChanged = "OriginalPrice",
                    OldValue = existing.OriginalPrice.ToString(),
                    NewValue = product.OriginalPrice.ToString(),
                    ProductCode = existing.ProductCode,
                    Action = "Updated",
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                });

            if (existing.Name != product.Name)
                histories.Add(new ProductHistory
                {
                    Id = Guid.NewGuid(),
                    ProductId = id,
                    FieldChanged = "Name",
                    OldValue = existing.Name,
                    NewValue = product.Name,
                    ProductCode = existing.ProductCode,
                    Action = "Updated",
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                });

            existing.Name = product.Name;
            existing.SellingPrice = product.SellingPrice;
            existing.OriginalPrice = product.OriginalPrice;
            existing.Quantity = product.Quantity;
            existing.CategoryId = product.CategoryId;
            existing.ImageUrl = product.ImageUrl;
            existing.Metric = product.Metric;
            existing.MinQuantity = product.MinQuantity;
            existing.UpdatedAt = DateTime.UtcNow;

            if (histories.Any())
                _context.ProductHistories.AddRange(histories);

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
    
            return product;
            
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsPageAsync(
            int page, int limit, string? status = null, string? search = null, int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    EF.Functions.ILike(p.Name, $"%{search}%") ||
                    EF.Functions.ILike(p.ProductCode, $"%{search}%"));

            if (!string.IsNullOrEmpty(status) && status != "All")
                query = query.Where(p => p.Status == status);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(String category)
        {
            var products = await _context.Products
                .Where(p => p.Category.Name.Contains(category))
                .ToListAsync();


            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByStatusAsync(string status)
        {

            var products = await _context.Products.ToListAsync();

            return products
                .Where(p => p.Status == status);
        }

     
    }

}