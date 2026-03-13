namespace ProductService.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductService.Services;
using ProductService.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;
    
     /// <summary>Gets all products</summary>
    /// <returns>All products or NotFound.</returns>   
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        => Ok(await _productService.GetAllProductsAsync());  
    
    /// <summary>Gets a product by ID.</summary>
    /// <param name="id">Product ID.</param>
    /// <returns>Product or NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);  
        if (product == null) return NotFound();
        return Ok(product);
    }
}