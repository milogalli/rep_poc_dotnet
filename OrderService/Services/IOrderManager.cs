namespace OrderService.Services;

using System.Text.Json;
using OrderService.Models;
using Microsoft.Extensions.Logging;

public interface IOrderManager
{
    Task<Order?> CreateOrderAsync(int productId, int quantity);
    IEnumerable<Order> GetAllOrders();
}

public class OrderManager : IOrderManager
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderManager> _logger;
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public OrderManager(HttpClient httpClient, ILogger<OrderManager> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public IEnumerable<Order> GetAllOrders() => _orders;

    public async Task<Order?> CreateOrderAsync(int productId, int quantity)
    {
        // 1. Call ProductService via REST
        // NOTE: Adjust the port based on ProductService's launchSettings.json!
        var response = await _httpClient.GetAsync($"http://localhost:5000/api/product/{productId}");
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to create order. Product {@ProductId} not found.", productId);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (product == null) return null;

        // 2. Business Logic
        var order = new Order
        {
            Id = _nextId++,
            ProductId = productId,
            Quantity = quantity,
            TotalPrice = product.Price * quantity
        };

        _orders.Add(order);

        // 3. Structured Logging
        _logger.LogInformation(
            "Order successfully created: {@OrderId} for {@ProductId} with {@Quantity}. Total: {@TotalPrice}", 
            order.Id, order.ProductId, order.Quantity, order.TotalPrice);

        return order;
    }
}