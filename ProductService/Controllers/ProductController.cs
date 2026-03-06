namespace ProductService.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductService.Services;
using ProductService.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll() => Ok(_productService.GetAllProducts());

    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}