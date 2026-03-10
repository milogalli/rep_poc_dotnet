namespace OrderService.Services;

using System.Text.Json;
using OrderService.Models;
using Microsoft.Extensions.Logging;

public interface IOrderManager
{
    Task<Order?> CreateOrderAsync(int productId, int quantity);
    IEnumerable<Order> GetAllOrders();
}

public class OrderManager(HttpClient httpClient, ILogger<OrderManager> logger) : IOrderManager
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<OrderManager> _logger = logger;
    private readonly List<Order> _orders = [];
    private int _nextId = 1;

    public IEnumerable<Order> GetAllOrders() => _orders;

    public async Task<Order?> CreateOrderAsync(int productId, int quantity)
    {
        var response = await _httpClient.GetAsync($"http://localhost:5000/api/product/{productId}");
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to create order. Product {@ProductId} not found.", productId);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (product == null) return null;

        var order = new Order
        {
            Id = _nextId++,
            ProductId = productId,
            Quantity = quantity,
            TotalPrice = product.Price * quantity
        };

        _orders.Add(order);

        _logger.LogInformation(
            "Order successfully created: {@OrderId} for {@ProductId} with {@Quantity}. Total: {@TotalPrice}", 
            order.Id, order.ProductId, order.Quantity, order.TotalPrice);

        return order;
    }
}