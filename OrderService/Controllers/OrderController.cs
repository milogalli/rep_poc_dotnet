namespace OrderService.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using OrderService.Models;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderManager orderManager) : ControllerBase
{
    private readonly IOrderManager _orderManager = orderManager;

    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetAll() => Ok(_orderManager.GetAllOrders());

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromQuery] int productId, [FromQuery] int quantity)
    {
        var order = await _orderManager.CreateOrderAsync(productId, quantity);
        if (order == null) return BadRequest("Invalid Product ID");
        
        return Ok(order);
    }
}