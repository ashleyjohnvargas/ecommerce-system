using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;

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
            _context.Users.Add(user);
            _context.SaveChanges();

            // Store user information in the session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
            HttpContext.Session.SetString("Address", user.Address ?? string.Empty);

             // Check if the email contains "@admin"
            if (user.Email.Contains("@admin"))
            {
                // Redirect to the desired controller and action for admin
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Redirect to the default Index action of Home controller
                return RedirectToAction("CustomerIndex", "Home");
            }
        }


        // Action for logging in the user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                // Store user information in the session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
                HttpContext.Session.SetString("Address", user.Address ?? string.Empty);

                // Check if the email contains "@admin"
                if (email.Contains("@admin"))
                {
                    // Redirect to the desired controller and action for admin
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Redirect to the default Index action of Home controller
                    return RedirectToAction("CustomerIndex", "Home");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password";
            return View("Authentication");
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

