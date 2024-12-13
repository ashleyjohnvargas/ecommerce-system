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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction("Success"); // Redirect to a success page
            }
            return View("Authentication");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var customer = _context.Customers
                .FirstOrDefault(c => c.Email == email && c.Password == password);

            if (customer != null)
            {
                // Logic for successful login
                return RedirectToAction("Index", "Home"); // Redirect to home
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View("Authentication");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}

