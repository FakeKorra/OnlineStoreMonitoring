using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMonitoring.Application.Services;

namespace OnlineStoreMonitoring.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("events-csv")]
        public async Task<ActionResult> ExportEventsCsv()
        {
            var csv = await _reportService.GenerateEventsCsvAsync();
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"events_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        [HttpGet("orders-csv")]
        public async Task<ActionResult> ExportOrdersCsv()
        {
            var csv = await _reportService.GenerateOrdersCsvAsync();
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"orders_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        [HttpGet("analytics-csv")]
        public async Task<ActionResult> ExportAnalyticsCsv()
        {
            var csv = await _reportService.GenerateAnalyticsCsvAsync();
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"analytics_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }
    }
}