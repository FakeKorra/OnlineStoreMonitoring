using System;

namespace OnlineStoreMonitoring.Application.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string Details { get; set; }
    }
}