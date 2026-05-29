using System;
using Microsoft.Extensions.Logging;

namespace OnlineStoreMonitoring.Infrastructure.Logging
{
    public interface ILoggingService
    {
        void LogUserLogin(string username, int userId);
        void LogProductCreated(int productId, string productName);
        void LogEventRecorded(int eventId, string eventType);
        void LogOrderCreated(int orderId, decimal totalAmount);
        void LogError(string message, Exception ex);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogUserLogin(string username, int userId)
        {
            _logger.LogInformation($"User login: {username} (ID: {userId}) at {DateTime.UtcNow}");
        }

        public void LogProductCreated(int productId, string productName)
        {
            _logger.LogInformation($"Product created: {productName} (ID: {productId})");
        }

        public void LogEventRecorded(int eventId, string eventType)
        {
            _logger.LogInformation($"Event recorded: {eventType} (ID: {eventId})");
        }

        public void LogOrderCreated(int orderId, decimal totalAmount)
        {
            _logger.LogInformation($"Order created: ID: {orderId}, Total: {totalAmount}");
        }

        public void LogError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
        }
    }
}