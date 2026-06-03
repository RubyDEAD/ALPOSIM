using alposim.Interfaces;
using alposim.Models;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers
{

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
        [ProducesResponseType(typeof(IEnumerable<Sale>), 200)]
        public async Task<IActionResult> GetSales()
        {
            var sales = await _saleRepository.GetSales();
            if (sales == null) return NotFound();
            
            return Ok(sales);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> GetSalebyId(Guid id)
        {
            var sale = await _saleRepository.GetSaleById(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("range")]
        [ProducesResponseType(typeof(IEnumerable<Sale>), 200)]
        public async Task<IActionResult> GetSales(DateTime startDate, DateTime endDate)
        {
            var sale = await _saleRepository.GetSalesFromDateRange(startDate, endDate);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("payment")]
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> GetSalesByPayment(bool payment)
        {
            var sale = await _saleRepository.GetSalesByPayment(payment);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Sale), 201)]
        public async Task<IActionResult> AddSale(Sale sale)
        {
            if(!ModelState.IsValid) return BadRequest();

            var newsale = await _saleRepository.CreateSale(sale);
            return Ok(newsale);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] Sale sale)
        {
            if(!ModelState.IsValid) return BadRequest();
            
            var updatesale = await _saleRepository.UpdateSale(id, sale);
            
            return Ok(updatesale);
        }

        [HttpDelete("{id}")]
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