using alposim.Interfaces;
using alposim.Models;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : Controller
    {
        private readonly ISaleRepository saleRepository;

        public SaleController(ISaleRepository saleRepository)
        {
            this.saleRepository = saleRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Sale>), 200)]
        public async Task<IActionResult> GetSales()
        {
            var sales = await saleRepository.GetSales();
            if (sales == null) return NotFound();
            
            return Ok(sales);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> GetSalebyId(Guid id)
        {
            var sale = await saleRepository.GetSaleById(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("range")]
        [ProducesResponseType(typeof(IEnumerable<Sale>), 200)]
        public async Task<IActionResult> GetSales(DateTime startDate, DateTime endDate)
        {
            var sale = await saleRepository.GetSalesFromDateRange(startDate, endDate);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet("payment")]
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> GetSalesByPayment(bool payment)
        {
            var sale = await saleRepository.GetSalesByPayment(payment);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Sale), 201)]
        public async Task<IActionResult> AddSale(Sale sale)
        {
            if(!ModelState.IsValid) return BadRequest();

            var newsale = await saleRepository.CreateSale(sale);
            return Ok(newsale);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Sale), 200)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] Sale sale)
        {
            if(!ModelState.IsValid) return BadRequest();
            
            var updatesale = await saleRepository.UpdateSale(id, sale);
            
            return Ok(updatesale);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]        
        public async Task<ActionResult> RemoveSale(Guid id)
        {
            var sale = await saleRepository.GetSaleById(id);
            if (sale == null) return NotFound();
            await saleRepository.DeleteSale(id);
            return NoContent();
        }
        
    }
}