using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMonitoring.Application.DTOs;
using OnlineStoreMonitoring.Application.Services;

namespace OnlineStoreMonitoring.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("track")]
        [Authorize]
        public async Task<ActionResult<EventDto>> TrackEvent(CreateEventDto createEventDto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var eventDto = await _eventService.RecordEventAsync(createEventDto, ipAddress, userAgent);
            return CreatedAtAction("GetEvent", new { id = eventDto.Id }, eventDto);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<EventDto>>> GetUserEvents(int userId)
        {
            var events = await _eventService.GetEventsByUserAsync(userId);
            return Ok(events);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<List<EventDto>>> GetProductEvents(int productId)
        {
            var events = await _eventService.GetEventsByProductAsync(productId);
            return Ok(events);
        }

        [HttpGet("type/{eventType}")]
        public async Task<ActionResult<List<EventDto>>> GetEventsByType(string eventType)
        {
            var events = await _eventService.GetEventsByTypeAsync(eventType);
            return Ok(events);
        }
    }
}