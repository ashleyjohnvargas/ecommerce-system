using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EcommerceSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OrderHistory()
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            // If the user is not logged in (no UserId in session), redirect to the login page
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the user from the database based on the user ID stored in the session
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(); // Return 404 if the user doesn't exist
            }

            Console.WriteLine($"Session Id: {userId}");
            Console.WriteLine($"User Id: {user.Id}");

            // Fetch orders for the logged-in user, ordered by CreatedAt (most recent first)
            var orders = _context.Orders
                .Where(o => o.CustomerId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            // For each order, manually fetch the order items using OrderId
            var orderHistoryViewModel = orders.Select(order => new OrderHistoryViewModel
            {
                OrderId = order.OrderId,
                CreatedAt = order.CreatedAt?.ToString("yyyy-MM-dd"), // Format the date
                OrderStatus = order.OrderStatus,
                ItemsCount = _context.OrderItems
                    .Where(oi => oi.OrderId == order.OrderId)
                    .Sum(oi => oi.Quantity.GetValueOrDefault()) // Safely convert to non-nullable int
            }).ToList();

            return View(orderHistoryViewModel); // Return the view with the order history model
        }

    }

}