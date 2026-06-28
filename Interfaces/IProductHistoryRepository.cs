using alposim.Models;

namespace alposim.Interfaces;

public interface IProductHistoryRepository
{
    Task<IEnumerable<ProductHistory>> GetProductHistoryAsync(Guid productId);
}