using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ApplicationDbContext _context;

        public CheckoutController(ILogger<CartController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Checkout()
        {
            // Step 1: Retrieve the user ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not logged in
            }

            // Step 2: Check for an active cart for the user
            var activeCart = _context.Carts.FirstOrDefault(c => c.CustomerId == userId && c.Status == "Active");

            if (activeCart == null)
            {
                // If no active cart exists, redirect to the cart page or show an empty cart message
                return RedirectToAction("CustomerCartIcon");
            }

            // Step 3: Retrieve the items in the cart
            var cartItems = _context.CartItems
                .Where(ci => ci.CartId == activeCart.CartId)
                .Include(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .Select(ci => new
                {
                    CartItemID = ci.CartItemId,
                    ProductId = ci.Product.Id,
                    ProductName = ci.Product.Name,
                    ProductDescription = ci.Product.Description,
                    ProductPrice = ci.Product.Price,
                    ProductQuantity = ci.Quantity,
                    ProductSubtotal = ci.Subtotal,
                    ImagePath = ci.Product.Images.FirstOrDefault().FilePath
                })
                .ToList();

            // Step 4: Prepare the model for the checkout view
            var checkoutModel = new
            {
                CartItems = cartItems,
                CartTotalPrice = activeCart.TotalPrice,
                ShippingFee = 0, // Assuming free shipping, you can adjust this as needed
                CartTotal = activeCart.TotalPrice // Adjust this if shipping is added
            };

            var checkoutViewModel = new CheckoutViewModel(); // Prepare the checkout view model

            ViewBag.CheckoutModel = checkoutViewModel; // Pass the checkout model to the view

            // Pass the model to the view
            return View(checkoutModel);
        }




        [HttpPost]
        public IActionResult PlaceOrder(CheckoutViewModel checkoutModel)
        {
            // Get the current user's ID from session
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "Account"); // If not logged in, redirect to login page
            }

            // Find the active cart for the logged-in user
            var activeCart = _context.Carts.Include(c => c.CartItems)
                                           .FirstOrDefault(c => c.CustomerId == userId && c.Status == "Active");

            if (activeCart == null)
            {
                return RedirectToAction("Index", "Home"); // If no active cart found, redirect to home page
            }

            // Create an Order instance and set its properties
            var order = new Order
            {
                CustomerId = userId,
                TotalPrice = activeCart.TotalPrice,
                PaymentMethod = checkoutModel.PaymentMethod,
                OrderStatus = "Pending",
                CreatedAt = DateTime.Now
            };

            // Add order to database and save changes
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Create OrderItems for each item in the active cart
            foreach (var cartItem in activeCart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Subtotal = cartItem.Subtotal
                };

                // Add each order item to the database
                _context.OrderItems.Add(orderItem);
            }
            _context.SaveChanges();

            // Create a Billing instance
            var billing = new Billing
            {
                OrderId = order.OrderId,
                CustomerId = userId,
                BillingDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(3),
                TotalAmount = order.TotalPrice,
                PaymentStatus = "Pending",
                BillingAddress = checkoutModel.Address,
                TaxAmount = 0, // Default value
                ShippingFee = 0, // Default value
                GrandTotal = order.TotalPrice, // Total + Tax + ShippingFee
                FirstName = checkoutModel.FirstName,
                LastName = checkoutModel.LastName,
                CompanyName = checkoutModel.CompanyName,
                PhoneNumber = checkoutModel.PhoneNumber,
                EmailAddress = checkoutModel.EmailAddress,
                PaymentMethod = checkoutModel.PaymentMethod
            };

            // Add the billing to the database and save changes
            _context.Billings.Add(billing);
            _context.SaveChanges();

            // Optional: Update the cart status to 'Inactive' or similar after the order is placed
            activeCart.Status = "Completed";
            _context.SaveChanges();

            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Your order has been placed successfully!";

            // Redirect to a confirmation page or order summary
            return RedirectToAction("CustomerIndex", "Home");
        }

    }
}
