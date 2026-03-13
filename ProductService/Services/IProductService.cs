using ProductService.Models;
using StackExchange.Redis;
using System.Text.Json;  // This is needed for JsonSerializer

namespace ProductService.Services;

public interface IProductService
{
    Task<Product?> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
}

public class ProductManager : IProductService
{
    private readonly ILogger<ProductManager> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
    private const string ProductKeyPrefix = "product:";

    // In-memory store for PoC
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.50m }
    };

    public ProductManager(ILogger<ProductManager> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
        _cache = redis.GetDatabase();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var cacheKey = "products:all";
        var cached = await _cache.StringGetAsync(cacheKey);

        if (!cached.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<List<Product>>(cached.ToString()) ?? _products;
        }

        _logger.LogInformation("Cache miss for all products");
        await _cache.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(_products),
            CacheExpiration
        );

        return _products;
    }


    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"{ProductKeyPrefix}{id}";

        var cachedProduct = await _cache.StringGetAsync(cacheKey);
        if (!cachedProduct.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<Product>(cachedProduct.ToString());
        }

        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} was not found.", id);
            return null;
        }

        await _cache.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            CacheExpiration
        );

        return product;
    }
   
}
