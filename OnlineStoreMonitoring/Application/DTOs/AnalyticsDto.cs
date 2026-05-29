namespace OnlineStoreMonitoring.Application.DTOs;

public class ConversionAnalyticsDto
{
    public int TotalViews { get; set; }
    public int TotalCartAdds { get; set; }
    public int TotalOrders { get; set; }
    public decimal ConversionRate { get; set; }
}

public class OrderStatusAnalyticsDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class PopularProductsDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int AddToCartCount { get; set; }
    public int OrderCount { get; set; }
}