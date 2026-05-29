using System;

namespace OnlineStoreMonitoring.Domain.Entities
{
    public class CustomerEvent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}