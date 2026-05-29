using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineStoreMonitoring.Application.DTOs;
using OnlineStoreMonitoring.Application.Repositories;
using OnlineStoreMonitoring.Domain.Entities;

namespace OnlineStoreMonitoring.Application.Services
{
    public interface IEventService
    {
        Task<EventDto> RecordEventAsync(CreateEventDto createEventDto, string ipAddress, string userAgent);
        Task<List<EventDto>> GetEventsByUserAsync(int userId);
        Task<List<EventDto>> GetEventsByProductAsync(int productId);
        Task<List<EventDto>> GetEventsByTypeAsync(string eventType);
        Task<ConversionAnalyticsDto> GetConversionAnalyticsAsync();
        Task<List<OrderStatusAnalyticsDto>> GetOrderStatusAnalyticsAsync();
        Task<List<PopularProductsDto>> GetPopularProductsAsync(int topCount = 10);
    }

    public class EventService : IEventService
    {
        private readonly IRepository<CustomerEvent> _eventRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;

        public EventService(
            IRepository<CustomerEvent> eventRepository,
            IRepository<Order> orderRepository,
            IRepository<Product> productRepository)
        {
            _eventRepository = eventRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<EventDto> RecordEventAsync(CreateEventDto createEventDto, string ipAddress, string userAgent)
        {
            var customerEvent = new CustomerEvent
            {
                UserId = createEventDto.UserId,
                ProductId = createEventDto.ProductId,
                EventType = createEventDto.EventType,
                EventDate = DateTime.UtcNow,
                Details = createEventDto.Details,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _eventRepository.AddAsync(customerEvent);
            return MapToDto(customerEvent);
        }

        public async Task<List<EventDto>> GetEventsByUserAsync(int userId)
        {
            var events = await _eventRepository.FindAsync(e => e.UserId == userId);
            return events.Select(MapToDto).ToList();
        }

        public async Task<List<EventDto>> GetEventsByProductAsync(int productId)
        {
            var events = await _eventRepository.FindAsync(e => e.ProductId == productId);
            return events.Select(MapToDto).ToList();
        }

        public async Task<List<EventDto>> GetEventsByTypeAsync(string eventType)
        {
            var events = await _eventRepository.FindAsync(e => e.EventType == eventType);
            return events.Select(MapToDto).ToList();
        }

        public async Task<ConversionAnalyticsDto> GetConversionAnalyticsAsync()
        {
            var allEvents = await _eventRepository.GetAllAsync();

            int totalViews = allEvents.Count(e => e.EventType == "ProductView");
            int totalCartAdds = allEvents.Count(e => e.EventType == "CartAdd");
            int totalOrders = allEvents.Count(e => e.EventType == "Order");

            decimal conversionRate = totalViews > 0 ? (decimal)totalOrders / totalViews : 0;

            return new ConversionAnalyticsDto
            {
                TotalViews = totalViews,
                TotalCartAdds = totalCartAdds,
                TotalOrders = totalOrders,
                ConversionRate = conversionRate
            };
        }

        public async Task<List<OrderStatusAnalyticsDto>> GetOrderStatusAnalyticsAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var statusGroups = orders.GroupBy(o => o.Status)
                .Select(g => new OrderStatusAnalyticsDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return statusGroups;
        }

        public async Task<List<PopularProductsDto>> GetPopularProductsAsync(int topCount = 10)
        {
            var allEvents = await _eventRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            var productAnalytics = products
                .Select(p => new PopularProductsDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ViewCount = allEvents.Count(e => e.ProductId == p.Id && e.EventType == "ProductView"),
                    AddToCartCount = allEvents.Count(e => e.ProductId == p.Id && e.EventType == "CartAdd"),
                    OrderCount = allEvents.Count(e => e.ProductId == p.Id && e.EventType == "Order")
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(topCount)
                .ToList();

            return productAnalytics;
        }

        private EventDto MapToDto(CustomerEvent customerEvent)
        {
            return new EventDto
            {
                Id = customerEvent.Id,
                UserId = customerEvent.UserId,
                ProductId = customerEvent.ProductId,
                EventType = customerEvent.EventType,
                EventDate = customerEvent.EventDate,
                Details = customerEvent.Details
            };
        }
    }
}