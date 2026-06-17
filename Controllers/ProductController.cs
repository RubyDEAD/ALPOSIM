using Microsoft.AspNetCore.Mvc;
using alposim.Models;
using alposim.Interfaces;
using alposim.Repository;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GetProductsByStatus(ProductStatus status)
        {
            var products = await productRepository.GetProductsByStatusAsync(status);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetProductsByCategory(String category)
        {
            
            var products = await productRepository.GetProductsByCategoryAsync(category);
            if(products == null) return BadRequest(products);
            
            return Ok(products);
            
        }
    }
}