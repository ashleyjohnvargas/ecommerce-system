using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem.Controllers;

public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly ApplicationDbContext _context;

    public CartController(ILogger<CartController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // This returns the customer cart page
    public IActionResult CustomerCart(int Id, int Quantity)
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
            // Create a new cart if none exists
            activeCart = new Cart
            {
                CustomerId = userId.Value,
                TotalPrice = 0,
                Status = "Active"
            };

            _context.Carts.Add(activeCart);
            _context.SaveChanges(); // Save to get the CartId
        }

        // Step 3: Search for the product in the Products table
        var product = _context.Products.FirstOrDefault(p => p.Id == Id && !p.IsDeleted && p.IsBeingSold);
        if (product == null)
        {
            return NotFound("The product does not exist or is unavailable.");
        }

        // Step 4: Check if the product is already in the cart
        var existingCartItem = _context.CartItems
            .FirstOrDefault(ci => ci.CartId == activeCart.CartId && ci.ProductId == product.Id);

        if (existingCartItem != null)
        {
            // Product already in cart, update its quantity and subtotal
            existingCartItem.Quantity = Quantity; 
            existingCartItem.Subtotal = existingCartItem.Quantity * product.Price; // Update subtotal

            _context.CartItems.Update(existingCartItem); // Update the existing CartItem
        }
        else
        {
            // Product not in the cart, create a new CartItem
            var cartItem = new CartItem
            {
                CartId = activeCart.CartId,
                ProductId = product.Id,
                Quantity = Quantity,
                Subtotal = product.Price * Quantity
            };

            _context.CartItems.Add(cartItem); // Add new CartItem
        }

        // Step 5: Save changes to CartItems table
        _context.SaveChanges();

        // Step 5: Update the TotalPrice of the active cart
        activeCart.TotalPrice = _context.CartItems
            .Where(ci => ci.CartId == activeCart.CartId)
            .Sum(ci => ci.Subtotal);

        _context.Carts.Update(activeCart);
        _context.SaveChanges();

        // Step 6: Prepare the model for the cart view
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

        var cartModel = new
        {
            CartItems = cartItems,
            CartTotalPrice = activeCart.TotalPrice
        };

        // Pass the model to the view
        return View(cartModel);
    }




    // Returns the CustomerCartIcon view when the cart icon is clicked
    public IActionResult CustomerCartIcon()
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
            // If no active cart exists, return a view with an empty cart model
            var emptyCartModel = new
            {
                CartItems = new List<object>(),
                CartTotalPrice = 0
            };
            return View(emptyCartModel);
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

        // Step 4: Prepare the model for the cart view
        var cartModel = new
        {
            CartItems = cartItems,
            CartTotalPrice = activeCart.TotalPrice
        };

        // Pass the model to the view
        return View(cartModel);
    }



    // Hard delete a cart item from the cart    
    public IActionResult RemoveItem(int id)
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
            // If no active cart exists, return to the cart page with a message
            return RedirectToAction("CustomerCart", "Cart");
        }

        // Step 3: Find the cart item by the CartItemId
        var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartItemId == id && ci.CartId == activeCart.CartId);
        if (cartItem == null)
        {
            return NotFound("The item was not found in your cart.");
        }

        // Step 4: Remove the cart item
        _context.CartItems.Remove(cartItem);
        _context.SaveChanges();

        // Step 5: Update the total price of the cart after removal
        activeCart.TotalPrice = _context.CartItems
            .Where(ci => ci.CartId == activeCart.CartId)
            .Sum(ci => ci.Subtotal);

        _context.Carts.Update(activeCart);
        _context.SaveChanges();

        // Step 6: Redirect back to the cart page
        return RedirectToAction("CustomerCartIcon");
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
