using alposim.Data;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.EntityFrameworkCore;

namespace alposim.Repository
{
    
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;
            return category;
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category == null) return null;

            return category;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null) return null;
            
            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}