using alposim.Models;

namespace alposim.Interfaces;

public interface IProductHistoryRepository
{
    Task<IEnumerable<ProductHistory>> GetProductHistories();
    Task<IEnumerable<ProductHistory>> GetProductHistoryAsync(Guid productId);
}