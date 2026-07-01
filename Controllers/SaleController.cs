using alposim.DTO;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : Controller
    {
        private readonly ISaleRepository _saleRepository;

        public SaleController(ISaleRepository saleRepository)
        {
            this._saleRepository = saleRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(typeof(IEnumerable<Sale>), 200)]
        public async Task<IActionResult> GetSales()
        {
            var sales = await _saleRepository.GetSales();
            if (sales == null) return NotFound();
            
            return Ok(sales);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> GetSalebyId(Guid id)
        {
            var sale = await _saleRepository.GetSaleById(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("range")]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(typeof(IEnumerable<Sale>), 200)]
        public async Task<IActionResult> GetSales(DateTime startDate, DateTime endDate)
        {
            var sale = await _saleRepository.GetSalesFromDateRange(startDate, endDate);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("payment")]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> GetSalesByPayment(bool payment)
        {
            var sale = await _saleRepository.GetSalesByPayment(payment);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(200, Type = typeof(PagedResult<Sale>))]
        public async Task<IActionResult> GetSalesByPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int limit = 15,
            [FromQuery] string? saleCode = null,
            [FromQuery] bool? payment = null, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            if(page < 1 || limit < 1) return BadRequest();

            var (items, totalCount) = await _saleRepository.GetSalesPageAync(
                page, limit, saleCode, payment, startDate, endDate);

            var result = new PagedResult<Sale>
            {
                Items = items,
                PageNumber = page,
                Limit = limit,
                TotalCount = totalCount
            };

            return Ok(result);
        }
        

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(typeof(Sale), 201)]
        public async Task<IActionResult> AddSale(Sale sale)
        {
            if(!ModelState.IsValid) return BadRequest();

            var newsale = await _saleRepository.CreateSale(sale);
            return Ok(newsale);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] Sale sale)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                var updateSale = await _saleRepository.UpdateSale(id, sale);
                return Ok(updateSale);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")] // for the meantime
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]        
        public async Task<ActionResult> RemoveSale(Guid id)
        {
            var sale = await _saleRepository.GetSaleById(id);
            if (sale == null) return NotFound();
            await _saleRepository.DeleteSale(id);
            return NoContent();
        }

        [HttpPost("{id}/item")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> AddItemToSale(Guid id, [FromBody] SaleItem saleItem)
        {
            try
            {
                await _saleRepository.AddItemAsync(id, saleItem);
                return Ok(new { message = "Item added successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}/item/{saleItemId}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> RemoveItemFromSale(Guid id, Guid saleItemId)
        {
            try
            {
                await _saleRepository.RemoveItemAsync(id, saleItemId);
                return Ok(new { message = "Item removed successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{id}/item/{saleItemId}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateItemQuantity(
            Guid id,
            Guid saleItemId,
            [FromBody] UpdateSaleItemQuantityDto dto)
        {
            try
            {
                await _saleRepository.UpdateItemQuantityAsync(id, saleItemId, dto.Quantity);
                return Ok(new { message = "Quantity updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
    }
}