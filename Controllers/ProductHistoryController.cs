using alposim.Interfaces;
using alposim.Models;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProductHistoryController : Controller
{
    private readonly IProductHistoryRepository _repository;

    public ProductHistoryController(IProductHistoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductHistory>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProductHistories()
    {
        var pH = await _repository.GetProductHistories();

        if (!pH.Any()) return NoContent();

        return Ok(pH);
    }

    [HttpGet("id")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProductHistory>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProductHistorybyProductId(Guid productId)
    {
        var history = await _repository.GetProductHistoryAsync(productId);

        if (!history.Any()) return NoContent();
        return Ok(history);
    }
}