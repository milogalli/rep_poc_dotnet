namespace ProductService.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductService.Services;
using ProductService.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll() 
        => Ok(await _productService.GetAllProductsAsync());  // Added await

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);  // Added await
        if (product == null) return NotFound();
        return Ok(product);
    }
}