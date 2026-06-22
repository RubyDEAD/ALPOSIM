
using alposim.Models;
namespace alposim.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> GetProductByNameAsync(string name);
        Task<IEnumerable<Product>> GetProductsByStatusAsync(string status);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsPageAsync(int page, int limit);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Guid id, Product product);
        Task<Product> DeleteProductAsync(Guid id);
        
    }
}