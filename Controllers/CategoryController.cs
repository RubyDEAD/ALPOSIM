using alposim.Models;
using alposim.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repository;

        public CategoryController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]

        public async Task<IActionResult> FetchCategoriesAsync()
        {
            var categories = await _repository.GetCategoriesAsync();
            return Ok(categories);
        }
        
        [HttpGet("name/{name}")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public async Task<IActionResult> FetchCategoryByName(string name)
        {
            var categories = await _repository.GetCategoryByNameAsync(name);
            return Ok(categories);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public async Task<IActionResult> FetchCategoryById(int id)
        {
            var categories = await _repository.GetCategoryByIdAsync(id);
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(201, Type = typeof(Category))]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest();

            var cat = await _repository.CreateCategoryAsync(category);
            
            return CreatedAtAction(nameof(FetchCategoryById), new { id = cat.Id }, cat);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            var existingCategory = await _repository.UpdateCategoryAsync(id, category);

            if (!ModelState.IsValid || existingCategory == null) return BadRequest();

            return Ok(existingCategory);

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _repository.DeleteCategoryAsync(id);
            
            if(category == null) return NotFound();
            return Ok(category);
        }
    }
}