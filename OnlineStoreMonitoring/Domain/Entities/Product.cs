using System;
using System.Collections.Generic;

namespace OnlineStoreMonitoring.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<CustomerEvent> CustomerEvents { get; set; } = new List<CustomerEvent>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}