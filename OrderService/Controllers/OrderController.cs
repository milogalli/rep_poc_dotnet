namespace OrderService.Controllers;

using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using OrderService.Models;
using MicroServicePoC.Utilities;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderManager) : ControllerBase
{
    private readonly IOrderService _orderManager = orderManager;

    /// <summary>
    /// Retrieves all orders from the system.
    /// </summary>
    /// <remarks>This method is typically used to display a list of orders in a user interface. Ensure that
    /// the calling context has the necessary permissions to access order data.</remarks>
    /// <returns>An <see cref="IEnumerable{Order}"/> containing all orders. The collection is empty if no orders exist.</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetAll() => Ok(_orderManager.GetAllOrders());

    /// <summary>
    /// Creates a new order for the specified product and quantity.
    /// </summary>
    /// <remarks>This method is asynchronous and should be awaited. It validates the product ID before
    /// creating the order.</remarks>
    /// <param name="productId">The unique identifier of the product to be ordered. Must be a valid product ID.</param>
    /// <param name="quantity">The number of units to order. Must be a positive integer.</param>
    /// <returns>An ActionResult containing the created Order object if successful; otherwise, a BadRequest result indicating an
    /// invalid product ID.</returns>
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromQuery] int productId, [FromQuery] int quantity)
    {
        if (ValidationHelpers.IsValidEmail("pippo@gmail.com"))
        {
            var order = await _orderManager.CreateOrderAsync(productId, quantity);
            if (order == null) return BadRequest("Invalid Product ID");

            return Ok(order);
        }
        return BadRequest("Email not validate");
    }
}
