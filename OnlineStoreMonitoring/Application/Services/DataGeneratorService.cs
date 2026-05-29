using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineStoreMonitoring.Application.Repositories;
using OnlineStoreMonitoring.Domain.Entities;

namespace OnlineStoreMonitoring.Application.Services
{
    public interface IDataGeneratorService
    {
        Task GenerateTestDataAsync(int userCount, int productCount, int eventCount);
    }

    public class DataGeneratorService : IDataGeneratorService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<CustomerEvent> _eventRepository;
        private readonly IAuthService _authService;

        private readonly List<string> _firstNames = new() { "Иван", "Петр", "Сергей", "Андрей", "Владимир", "Виктор", "Олег", "Евгений", "Дмитрий", "Алексей" };
        private readonly List<string> _lastNames = new() { "Иванов", "Петров", "Сидоров", "Смирнов", "Орлов", "Соколов", "Морозов", "Волков", "Назаров", "Матвеев" };
        private readonly List<string> _categories = new() { "Electronics", "Books", "Clothing", "Food", "Home", "Sports", "Toys", "Beauty" };
        private readonly List<string> _productNames = new() { "Laptop", "Smartphone", "Tablet", "Headphones", "Monitor", "Keyboard", "Mouse", "Printer", "Router", "Camera" };
        private readonly List<string> _eventTypes = new() { "ProductView", "CartAdd", "Order" };

        public DataGeneratorService(
            IUserRepository userRepository,
            IRepository<Product> productRepository,
            IRepository<CustomerEvent> eventRepository,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _eventRepository = eventRepository;
            _authService = authService;
        }

        public async Task GenerateTestDataAsync(int userCount, int productCount, int eventCount)
        {
            await GenerateUsersAsync(userCount);
            await GenerateProductsAsync(productCount);
            await GenerateEventsAsync(eventCount);
        }

        private async Task GenerateUsersAsync(int count)
        {
            var random = new Random();
            for (int i = 1; i <= count; i++)
            {
                var firstName = _firstNames[random.Next(_firstNames.Count)];
                var lastName = _lastNames[random.Next(_lastNames.Count)];
                var username = $"{firstName.ToLower()}{lastName.ToLower()}{i}";
                var email = $"{username}@example.com";

                var existingUser = await _userRepository.GetByUsernameAsync(username);
                if (existingUser != null) continue;

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = _authService.HashPassword("password123"),
                    Role = i <= 2 ? "Admin" : "Customer",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(365)),
                    LastLoginAt = DateTime.UtcNow.AddDays(-random.Next(30))
                };

                await _userRepository.AddAsync(user);
            }
        }

        private async Task GenerateProductsAsync(int count)
        {
            var random = new Random();
            var products = new List<Product>();

            for (int i = 1; i <= count; i++)
            {
                var product = new Product
                {
                    Name = $"{_productNames[random.Next(_productNames.Count)]} #{i}",
                    Description = $"High quality product number {i}",
                    Price = (decimal)(9.99 + random.Next(1, 1000)),
                    StockQuantity = random.Next(10, 1000),
                    Category = _categories[random.Next(_categories.Count)],
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(365))
                };
                products.Add(product);
            }

            await _productRepository.AddRangeAsync(products);
        }

        private async Task GenerateEventsAsync(int count)
        {
            var random = new Random();
            var users = await _userRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            if (!users.Any() || !products.Any()) return;

            var userList = users.ToList();
            var productList = products.ToList();
            var events = new List<CustomerEvent>();

            for (int i = 0; i < count; i++)
            {
                var user = userList[random.Next(userList.Count)];
                var product = productList[random.Next(productList.Count)];

                var customerEvent = new CustomerEvent
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    EventType = _eventTypes[random.Next(_eventTypes.Count)],
                    EventDate = DateTime.UtcNow.AddDays(-random.Next(90)),
                    Details = "Generated test event",
                    IpAddress = $"192.168.1.{random.Next(1, 255)}",
                    UserAgent = "Mozilla/5.0 (Test)"
                };
                events.Add(customerEvent);
            }

            await _eventRepository.AddRangeAsync(events);
        }
    }
}