using System.Net;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;
using OrderService.Services;

namespace OrderService.Tests;

public class OrderManagerTests
{
    [Fact]
    public async Task CreateOrderAsync_ValidProduct_ReturnsOrder()
    {
        // Arrange
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Id\":1,\"Name\":\"Laptop\",\"Price\":1200.00}")
            });

        var client = new HttpClient(mockMessageHandler.Object);
        var mockLogger = new Mock<ILogger<OrderManager>>();
        var service = new OrderManager(client, mockLogger.Object);

        // Act
        var result = await service.CreateOrderAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2500.00m, result.TotalPrice);
    }
}