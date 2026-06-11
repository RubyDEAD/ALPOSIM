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
            if(!ModelState.IsValid) return BadRequest();
            
            var updatesale = await _saleRepository.UpdateSale(id, sale);
            
            return Ok(updatesale);
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
        
    }
}