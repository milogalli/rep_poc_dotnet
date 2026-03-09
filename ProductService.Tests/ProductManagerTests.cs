using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ProductService.Services;
using ProductService.Models;

namespace ProductService.Tests;

public class ProductManagerTests
{
    private readonly Mock<ILogger<ProductManager>> _loggerMock;
    private readonly ProductManager _productManager;

    public ProductManagerTests()
    {
        _loggerMock = new Mock<ILogger<ProductManager>>();
        _productManager = new ProductManager(_loggerMock.Object);
    }

    [Fact]
    public void GetAllProducts_ReturnsAllProducts()
    {
        // Act
        var result = _productManager.GetAllProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, p => p.Name == "Laptop");
        Assert.Contains(result, p => p.Name == "Mouse");
    }

    [Theory]
    [InlineData(1, "Laptop", 1200.00)]
    [InlineData(2, "Mouse", 25.50)]
    public void GetProductById_ExistingId_ReturnsProduct(int id, string expectedName, decimal expectedPrice)
    {
        // Act
        var result = _productManager.GetProductById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedName, result.Name);
        Assert.Equal(expectedPrice, result.Price);
    }
}