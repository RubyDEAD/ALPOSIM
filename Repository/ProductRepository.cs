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

        public async Task<Product> UpdateProductAsync(Guid id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return null;
            }
            existingProduct.Name = product.Name;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Quantity = product.Quantity;
            existingProduct.OriginalPrice = product.OriginalPrice;
            existingProduct.SellingPrice = product.SellingPrice;
            existingProduct.Metric = product.Metric;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.MinQuantity = product.MinQuantity;
            await _context.SaveChangesAsync();
            return existingProduct;

        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
    
            return product;
            
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsPageAsync(int page, int limit)
        {
            var query = _context.Products.AsQueryable();
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