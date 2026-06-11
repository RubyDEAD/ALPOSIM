using alposim.Models;

namespace alposim.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryByNameAsync(string name);
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    
    Task<Category> UpdateCategoryAsync(int id, Category category);
    Task <Category> DeleteCategoryAsync(int id);
}