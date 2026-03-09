using System.Net;
using System.Text.Json;
using System.Threading.Tasks;  // Required for Task
using Moq.Protected;           // Required for Protected() setup
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OrderService.Services;
using OrderService.Models;

namespace OrderService.Tests;

public class OrderManagerTests
{
    private readonly Mock<ILogger<OrderManager>> _loggerMock;
    private readonly Mock<System.Net.Http.HttpMessageHandler> _httpMessageHandlerMock;  // Fully qualified
    private readonly System.Net.Http.HttpClient _httpClient;  // Fully qualified
    private readonly OrderManager _orderManager;

    public OrderManagerTests()
    {
        _loggerMock = new Mock<ILogger<OrderManager>>();
        _httpMessageHandlerMock = new Mock<System.Net.Http.HttpMessageHandler>();
        
        _httpClient = new System.Net.Http.HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new System.Uri("http://localhost:5000/")
        };
        
        _orderManager = new OrderManager(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateOrderAsync_ValidProduct_ReturnsOrder()  // Fully qualified
    {
        // Arrange
        var productDto = new ProductDto { Id = 1, Name = "Laptop", Price = 1200.00m };
        var jsonResponse = JsonSerializer.Serialize(productDto);
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage>>(  // Fully qualified
                "SendAsync",
                Moq.Protected.ItExpr.Is<System.Net.Http.HttpRequestMessage>(req => 
                    req.Method == System.Net.Http.HttpMethod.Get && 
                    req.RequestUri.ToString().Contains("/api/product/1")),
                Moq.Protected.ItExpr.IsAny<System.Threading.CancellationToken>())
            .ReturnsAsync(new System.Net.Http.HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new System.Net.Http.StringContent(jsonResponse)
            });

        // Act
        var result = await _orderManager.CreateOrderAsync(1, 2);

        // Assert
        Xunit.Assert.NotNull(result);
        Xunit.Assert.Equal(1, result.Id);
        Xunit.Assert.Equal(2400.00m, result.TotalPrice);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateOrderAsync_ProductNotFound_ReturnsNull()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage>>(
                "SendAsync",
                Moq.Protected.ItExpr.IsAny<System.Net.Http.HttpRequestMessage>(),
                Moq.Protected.ItExpr.IsAny<System.Threading.CancellationToken>())
            .ReturnsAsync(new System.Net.Http.HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        // Act
        var result = await _orderManager.CreateOrderAsync(999, 1);

        // Assert
        Xunit.Assert.Null(result);
    }
}