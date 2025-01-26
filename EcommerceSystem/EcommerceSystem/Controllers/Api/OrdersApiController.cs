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


        // ROute: api/OrdersApi/UpdateOrder
        [HttpPost("UpdateOrder")]
        public IActionResult UpdateOrder([FromBody] OrderRefundModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            // Find the order in the Ecommerce system by OrderId
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == model.OrderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // Update the OrderStatus and CreatedAt in the Ecommerce system
            order.OrderStatus = "Refunded";
            order.CreatedAt = model.RefundDate;

            // Save changes to the database
            _context.SaveChanges();

            return Ok("Order updated successfully.");
        }



        [HttpPost("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Update the order status
            order.OrderStatus = "Cancelled";
            order.IsDeleted = true;
            
            // Save changes in the Ecommerce database
            await _context.SaveChangesAsync();

            return Ok();
        }




    }
}