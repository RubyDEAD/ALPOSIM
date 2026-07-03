using alposim.Data;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.EntityFrameworkCore;

namespace alposim.Repository;

public class ProductHistoryRepository : IProductHistoryRepository
{
    private readonly LocalDbContext _context;

    public ProductHistoryRepository(LocalDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<ProductHistory>> GetProductHistories()
    {
        return await _context.ProductHistories
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductHistory>> GetProductHistoryAsync(Guid productId)
    {
        return await _context.ProductHistories
            .Where(p => p.ProductId == productId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();

      
    }
}