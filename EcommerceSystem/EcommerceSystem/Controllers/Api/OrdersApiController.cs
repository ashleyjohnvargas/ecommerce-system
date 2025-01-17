using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Controllers.Api 
{
    [Route("api/[controller]")]
    [ApiController] // Specifies that this is an API controller
    public class OrdersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllOrderItems/{orderId}")]
        public async Task<IActionResult> GetAllOrderItems(int orderId)
        {
            // Fetch the order items from the database using the OrderId
            var orderItems = await _context.OrderItems
                .Where(item => item.OrderId == orderId)
                .Select(item => new OrderItemCopy
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Subtotal = item.Subtotal
                })
                .ToListAsync();

            // Return the list of order items
            if (orderItems.Any())
            {
                return Ok(orderItems);
            }

            // Return 404 if no items are found
            return NotFound($"No items found for OrderId: {orderId}");
        }



    }
}