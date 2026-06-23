using Microsoft.AspNetCore.Mvc;
using alposim.Models;
using alposim.Interfaces;
using alposim.Repository;
using Microsoft.AspNetCore.Authorization;
using alposim.DTO;
namespace alposim.Controllers  
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;

        public ProductController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        public async Task<IActionResult> GetProducts()
        {
            var products = await productRepository.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(Product))]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var prod = await productRepository.GetProductByIdAsync(id);
            if (prod == null) return NotFound();


            return Ok(prod);
        }

        [HttpGet("name/{name}")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        public async Task<IActionResult> SearchProducts(string name)
        {
            var prod = await productRepository.GetProductByNameAsync(name);
            if (prod == null) return NotFound();
            return Ok(prod);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(201, Type = typeof(Product))]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {

            if (!ModelState.IsValid || product == null)
            {
                return BadRequest(ModelState);
            }

            var prod = await productRepository.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProductById), new { id = prod.Id }, prod);
        }
        
        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(PagedResult<Product>))]
        public async Task<IActionResult> GetProductsPaged(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null)
        {
            if (page < 1 || limit < 1)
                return BadRequest("Page and limit must be greater than 0.");

            var (items, totalCount) = await productRepository.GetProductsPageAsync(
                page, limit, status, search, categoryId);

            var result = new PagedResult<Product>
            {
                Items = items,
                PageNumber = page,
                Limit = limit,
                TotalCount = totalCount
            };

            return Ok(result);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedProduct = await productRepository.UpdateProductAsync(id, product);
            if (updatedProduct == null) return NotFound();


            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var deletedProduct = await productRepository.DeleteProductAsync(id);
            if (deletedProduct == null) return NotFound();

            return Ok(deletedProduct);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetProductsByStatus([FromRoute] string status)
        {
            if (!new[] { "Critical", "Low", "Normal", "High" }.Contains(status))
                return BadRequest("Invalid status. Use: Critical, Low, Normal, High");
        
            var products = await productRepository.GetProductsByStatusAsync(status);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            
            var products = await productRepository.GetProductsByCategoryAsync(category);
            if(products == null) return BadRequest(products);
            
            return Ok(products);
            
        }
    }
}