using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMonitoring.Application.DTOs;
using OnlineStoreMonitoring.Application.Services;

namespace OnlineStoreMonitoring.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ICacheService _cacheService;

    public AnalyticsController(IEventService eventService, ICacheService cacheService)
    {
        _eventService = eventService;
        _cacheService = cacheService;
    }

    [HttpGet("conversion")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetConversionAnalytics()
    {
        var cacheKey = "conversion_analytics";
        var cached = _cacheService.Get<ConversionAnalyticsDto>(cacheKey);

        if (cached != null)
        {
            return Ok(cached);
        }

        var result = await _eventService.GetConversionAnalyticsAsync();
        _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(10));

        return Ok(result);
    }

    [HttpGet("order-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderStatusAnalytics()
    {
        var cacheKey = "order_status_analytics";
        var cached = _cacheService.Get<List<OrderStatusAnalyticsDto>>(cacheKey);

        if (cached != null)
        {
            return Ok(cached);
        }

        var result = await _eventService.GetOrderStatusAnalyticsAsync();
        _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(10));

        return Ok(result);
    }

    [HttpGet("popular-products")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPopularProducts([FromQuery] int topCount = 10)
    {
        var cacheKey = $"popular_products_{topCount}";
        var cached = _cacheService.Get<List<PopularProductsDto>>(cacheKey);

        if (cached != null)
        {
            return Ok(cached);
        }

        var result = await _eventService.GetPopularProductsAsync(topCount);
        _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(10));

        return Ok(result);
    }
}