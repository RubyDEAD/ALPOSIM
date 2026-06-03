using alposim.DTO;
using alposim.Interfaces;
using alposim.Models;
using Microsoft.AspNetCore.Mvc;

namespace alposim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {

        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("daily")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), 200)]
        public async Task<IActionResult> GetDailyReports([FromQuery] DateTime date)
        {
            var report = await _reportRepository.GetDailyReportsAsync(date);
            if (report == null) return null;

            return Ok(report);
        }

        [HttpGet("weekly")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), 200)]
        public async Task<IActionResult> GetWeeklyReports([FromQuery] int month, int year)
        {
            var report = await _reportRepository.GetWeeklyReportsAsync(month, year);
            if (report == null) return null;
            return Ok(report);
        }

        [HttpGet("monthly")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), 200)]
        public async Task<IActionResult> GetMonthlyReports([FromQuery] int year)
        {
            var report = await _reportRepository.GetMonthlyReportsAsync(year);
            if (report == null) return null;
            return Ok(report);
        }

        [HttpGet("yearly")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), 200)]
        public async Task<IActionResult> GetYearlyReports()
        {
            var report = await _reportRepository.GetYearlyReportsAsync();
            if (report == null) return null;
            return Ok(report);
        }

    }
}