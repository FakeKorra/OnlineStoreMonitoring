using System;
using System.Collections.Generic;

namespace OnlineStoreMonitoring.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }

        public ICollection<CustomerEvent> CustomerEvents { get; set; } = new List<CustomerEvent>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}