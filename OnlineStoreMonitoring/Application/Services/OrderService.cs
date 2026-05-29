using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineStoreMonitoring.Application.DTOs;
using OnlineStoreMonitoring.Application.Repositories;
using OnlineStoreMonitoring.Domain.Entities;

namespace OnlineStoreMonitoring.Application.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<List<OrderDto>> GetOrdersByUserAsync(int userId);
        Task<List<OrderDto>> GetOrdersByStatusAsync(string status);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, string newStatus);
    }

    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in createOrderDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null) continue;

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    Total = product.Price * itemDto.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.Total;
            }

            var order = new Order
            {
                UserId = createOrderDto.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending",
                ShippingAddress = createOrderDto.ShippingAddress
            };

            await _orderRepository.AddAsync(order);

            foreach (var orderItem in orderItems)
            {
                orderItem.OrderId = order.Id;
                await _orderItemRepository.AddAsync(orderItem);
            }

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            var orderItems = await _orderItemRepository.FindAsync(oi => oi.OrderId == id);
            return MapToDto(order, orderItems.ToList());
        }

        public async Task<List<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _orderRepository.FindAsync(o => o.UserId == userId);
            var dtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderItems = await _orderItemRepository.FindAsync(oi => oi.OrderId == order.Id);
                dtos.Add(MapToDto(order, orderItems.ToList()));
            }

            return dtos;
        }

        public async Task<List<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _orderRepository.FindAsync(o => o.Status == status);
            var dtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderItems = await _orderItemRepository.FindAsync(oi => oi.OrderId == order.Id);
                dtos.Add(MapToDto(order, orderItems.ToList()));
            }

            return dtos;
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            order.Status = newStatus;
            if (newStatus == "Delivered")
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            await _orderRepository.UpdateAsync(order);
            return await GetOrderByIdAsync(orderId);
        }

        private OrderDto MapToDto(Order order, List<OrderItem> items)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = items.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}