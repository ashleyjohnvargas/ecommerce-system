using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceSystem.Services;

namespace EcommerceSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly PosService _posService;

        public CheckoutController(ILogger<CartController> logger, ApplicationDbContext context, PosService posService)
        {
            _logger = logger;
            _context = context;
            _posService = posService;
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




        
        // public IActionResult PlaceOrder(CheckoutViewModel checkoutModel)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel checkoutModel)
        {
            if (checkoutModel.FirstName == null || checkoutModel.LastName == null || checkoutModel.Address == null
                || checkoutModel.PhoneNumber == null || checkoutModel.EmailAddress == null || checkoutModel.PaymentMethod == null)
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Please input the required values";
                return RedirectToAction("Checkout");
            }

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

            // Set OrderStatus based on PaymentMethod
            string orderStatus = (checkoutModel.PaymentMethod == "E-Wallet" || checkoutModel.PaymentMethod == "Bank") 
                ? "Order Confirmed" 
                : "Order Placed";

            // Create an Order instance and set its properties
            var order = new Order
            {
                CustomerId = userId,
                TotalPrice = activeCart.TotalPrice,
                PaymentMethod = checkoutModel.PaymentMethod,
                OrderStatus = orderStatus,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            // Check if the Order entity is already tracked in the context
            var existingOrder = _context.Orders.Local.FirstOrDefault(o => o.OrderId == order.OrderId);
            if (existingOrder == null)
            {
                _context.Orders.Add(order);  // Add only if not tracked
            }
            _context.SaveChanges();  // Save order (this ensures the OrderId is set)

            // Create order items
            var orderItems = activeCart.CartItems.Select(cartItem => new OrderItem
            {
                // OrderItemId = 0, // Reset ID to avoid conflicts
                OrderId = order.OrderId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Subtotal = cartItem.Subtotal
            }).ToList();
            _context.OrderItems.AddRange(orderItems);
            _context.SaveChanges();

            // Map OrderItems to OrderItemDTO
            var orderItemDTOs = orderItems.Select(orderItem => new OrderItemCopy
            {
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                Subtotal = orderItem.Subtotal
            }).ToList();

            string paymentStatus = (checkoutModel.PaymentMethod == "E-Wallet" || checkoutModel.PaymentMethod == "Bank") 
                ? "Paid" 
                : "Pending";

            // Create a Billing instance
            var billing = new Billing
            {
                OrderId = order.OrderId,
                CustomerId = userId,
                BillingDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(3),
                TotalAmount = order.TotalPrice,
                PaymentStatus = paymentStatus,
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

             // Sync products with POS
            var productDtos = _context.Products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Color = p.Color,
                Category = p.Category,
                OriginalStock = p.OriginalStock,
                CurrentStock = p.CurrentStock,
                StockStatus = p.StockStatus,
                IsBeingSold = p.IsBeingSold,
                IsDeleted = p.IsDeleted,
                DateAdded = p.DateAdded
            }).ToList();
            
            await _posService.SyncProducts(productDtos); // POS Service 5 : Done



            // Send customer data to POS
            var customer = new Customer
            {
                CustomerId = userId,
                CustomerName = $"{checkoutModel.FirstName} {checkoutModel.LastName}",
                Address = checkoutModel.Address,
                PhoneNumber = checkoutModel.PhoneNumber,
                Email = checkoutModel.EmailAddress
            };
            await _posService.SyncCustomer(customer); // POS Service 1 : Done
            await _posService.CreateOrder(order); // POS Service 2 : Done
            await _posService.CreateOrderItems(orderItemDTOs); // POS Service 3 : Done

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                CustomerId = userId,
                SaleDate = DateTime.Now,
                TotalAmount = order.TotalPrice,
                PaymentStatus = paymentStatus,
                PaymentMethod = checkoutModel.PaymentMethod
            };
            await _posService.CreateInvoice(invoice); // POS Service 4 : Done

           
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
