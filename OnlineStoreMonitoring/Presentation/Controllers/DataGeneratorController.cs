using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMonitoring.Application.Services;

namespace OnlineStoreMonitoring.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DataGeneratorController : ControllerBase
    {
        private readonly IDataGeneratorService _dataGeneratorService;

        public DataGeneratorController(IDataGeneratorService dataGeneratorService)
        {
            _dataGeneratorService = dataGeneratorService;
        }

        [HttpPost("generate")]
        public async Task<ActionResult> GenerateTestData(
            [FromQuery] int userCount = 50,
            [FromQuery] int productCount = 100,
            [FromQuery] int eventCount = 1000)
        {
            await _dataGeneratorService.GenerateTestDataAsync(userCount, productCount, eventCount);
            return Ok(new { message = "Test data generated successfully" });
        }
    }
}