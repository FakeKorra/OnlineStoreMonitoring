using System.Collections.Generic;

namespace OnlineStoreMonitoring.Application.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}