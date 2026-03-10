namespace OrderService.Services;

using System.Text.Json;
using OrderService.Models;
using Microsoft.Extensions.Logging;
using OrderService.Controllers;


public interface IOrderService
{
    Task<Order?> CreateOrderAsync(int productId, int quantity);
    IEnumerable<Order> GetAllOrders();
}

public class OrderManager(HttpClient httpClient,IConfiguration configuration, ILogger<OrderController> logger) : IOrderService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<OrderController> _logger = logger;
    private readonly List<Order> _orders = [];
    private readonly string _baseUrl = configuration["ProductService:BaseUrl"] ?? throw new InvalidOperationException("ProductService:BaseUrl not configured");
    private int _nextId = 1;

    public IEnumerable<Order> GetAllOrders() => _orders;

  
    public async Task<Order?> CreateOrderAsync(int productId, int quantity)
    {
        
         var response = await _httpClient.GetAsync($"{_baseUrl}/api/product/{productId}");
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to create order. Product {@ProductId} not found.", productId);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductDto>(
            content, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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