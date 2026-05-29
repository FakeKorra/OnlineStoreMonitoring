using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStoreMonitoring.Application.DTOs;
using OnlineStoreMonitoring.Application.Repositories;
using OnlineStoreMonitoring.Domain.Entities;

namespace OnlineStoreMonitoring.Application.Services
{
    public interface IReportService
    {
        Task<string> GenerateEventsCsvAsync();
        Task<string> GenerateOrdersCsvAsync();
        Task<string> GenerateAnalyticsCsvAsync();
    }

    public class ReportService : IReportService
    {
        private readonly IRepository<CustomerEvent> _eventRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IEventService _eventService;

        public ReportService(
            IRepository<CustomerEvent> eventRepository,
            IRepository<Order> orderRepository,
            IEventService eventService)
        {
            _eventRepository = eventRepository;
            _orderRepository = orderRepository;
            _eventService = eventService;
        }

        public async Task<string> GenerateEventsCsvAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            var sb = new StringBuilder();

            sb.AppendLine("ID,UserId,ProductId,EventType,EventDate,Details,IpAddress");

            foreach (var evt in events)
            {
                sb.AppendLine($"{evt.Id},{evt.UserId},{evt.ProductId},{evt.EventType},{evt.EventDate:yyyy-MM-dd HH:mm:ss},{EscapeCsv(evt.Details)},{evt.IpAddress}");
            }

            return sb.ToString();
        }

        public async Task<string> GenerateOrdersCsvAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var sb = new StringBuilder();

            sb.AppendLine("ID,UserId,OrderDate,TotalAmount,Status,CompletedAt,ShippingAddress");

            foreach (var order in orders)
            {
                sb.AppendLine($"{order.Id},{order.UserId},{order.OrderDate:yyyy-MM-dd HH:mm:ss},{order.TotalAmount.ToString("F2", CultureInfo.InvariantCulture)},{order.Status},{(order.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")},{EscapeCsv(order.ShippingAddress)}");
            }

            return sb.ToString();
        }

        public async Task<string> GenerateAnalyticsCsvAsync()
        {
            var conversion = await _eventService.GetConversionAnalyticsAsync();
            var orderStatus = await _eventService.GetOrderStatusAnalyticsAsync();
            var popularProducts = await _eventService.GetPopularProductsAsync(10);

            var sb = new StringBuilder();

            sb.AppendLine("CONVERSION ANALYTICS");
            sb.AppendLine("Metric,Value");
            sb.AppendLine($"Total Views,{conversion.TotalViews}");
            sb.AppendLine($"Total Cart Adds,{conversion.TotalCartAdds}");
            sb.AppendLine($"Total Orders,{conversion.TotalOrders}");
            sb.AppendLine($"Conversion Rate,{conversion.ConversionRate:P2}");
            sb.AppendLine();

            sb.AppendLine("ORDER STATUS DISTRIBUTION");
            sb.AppendLine("Status,Count");
            foreach (var item in orderStatus)
            {
                sb.AppendLine($"{item.Status},{item.Count}");
            }
            sb.AppendLine();

            sb.AppendLine("POPULAR PRODUCTS");
            sb.AppendLine("ProductId,ProductName,Views,CartAdds,Orders");
            foreach (var item in popularProducts)
            {
                sb.AppendLine($"{item.ProductId},{EscapeCsv(item.ProductName)},{item.ViewCount},{item.AddToCartCount},{item.OrderCount}");
            }

            return sb.ToString();
        }

        private string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}