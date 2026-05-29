namespace OnlineStoreMonitoring.Application.DTOs
{
    public class CreateEventDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string EventType { get; set; }
        public string Details { get; set; }
    }
}