using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using BCrypt.Net;


namespace EcommerceSystem.Controllers 
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Authentication()
        {
            return View();
        }

        // Action for storing the user's data after registration or sign up
        public IActionResult Register(User user)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.UserName))
            {
                ModelState.AddModelError("", "Username, Email, and Password are required.");
                return View("Authentication", user); // Return with validation error
            }

            // Convert email to lowercase for consistent checks
            user.Email = user.Email.ToLower();

            // Check if email already exists (case-insensitive)
            if (_context.Users.Any(u => u.Email.ToLower() == user.Email))
            {
                ModelState.AddModelError("", "Email is already in use.");
                return View("Authentication", user);
            }

            // Assign default role based on email
            user.UserRole = user.Email.Contains("@admin") ? "Admin" : "Customer";

            // Securely hash the password before storing it
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Save user to database
            _context.Users.Add(user);
            _context.SaveChanges();

            // Store user information in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
            HttpContext.Session.SetString("Address", user.Address ?? string.Empty);
            HttpContext.Session.SetString("UserRole", user.UserRole);

            // Redirect based on role
            return user.UserRole == "Admin"
                ? RedirectToAction("Product", "Product")   // Admin dashboard
                : RedirectToAction("CustomerIndex", "Home"); // Customer homepage
        }


        // Action for logging in the user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Email and Password are required.";
                return View("Authentication");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))  // Secure password check
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("Authentication");
            }

            // Store user information in the session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
            HttpContext.Session.SetString("Address", user.Address ?? string.Empty);

            // Use a role-based check instead of checking for "@admin" in email
            if (user.UserRole == "Admin")
            {
                return RedirectToAction("Product", "Product");
            }

            return RedirectToAction("CustomerIndex", "Home");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Authentication");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}

