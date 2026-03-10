namespace ProductService.Services;

using ProductService.Models;
using Microsoft.Extensions.Logging;

public interface IProductService
{
    Product? GetProductById(int id);
    IEnumerable<Product> GetAllProducts();
}

public class ProductManager(ILogger<ProductManager> logger) : IProductService
{
    private readonly ILogger<ProductManager> _logger = logger;
   
    // In-memory store for PoC
    private readonly List<Product> _products =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.50m }
    ];

    public IEnumerable<Product> GetAllProducts() => _products;

    public Product? GetProductById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {@ProductId} was not found.", id);
        }
        return product;
    }
}