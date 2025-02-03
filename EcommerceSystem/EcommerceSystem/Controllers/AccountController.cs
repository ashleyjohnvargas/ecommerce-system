using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceSystem.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;


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
                TempData["ShowRegister"] = true; // Keeps user on the register panel
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Username, Email, and Password are required.";
                ModelState.AddModelError("", "Username, Email, and Password are required.");
                return View("Authentication", user); // Return with validation error
            }

            if (user.Password.Length < 15 && user.Password.Length < 64)
            {
                TempData["ShowRegister"] = true; // Keeps user on the register panel
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Password must be greater than or equal to 15 characters.";
                ModelState.AddModelError("", "Username, Email, and Password are required.");
                return View("Authentication", user); // Return with validation error
            }
            // Convert email to lowercase for consistent checks
            user.Email = user.Email.ToLower();

            // Check if email already exists (case-insensitive)
            if (_context.Users.Any(u => u.Email.ToLower() == user.Email))
            {
                TempData["ShowRegister"] = true; // Keeps user on the register panel
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Email is already in use.";
                ModelState.AddModelError("", "Email is already in use.");
                return View("Authentication", user);
            }

            // Assign default role based on email
            user.UserRole = user.Email.Contains("@admin") ? "Admin" : "Customer";

            // Securely hash the password before storing it
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            // Store user information in the session
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
            //// Check if the email contains "@admin"
            //if (user.Email.Contains("@admin"))
            //{
            //    // Redirect to the desired controller and action for admin
            //    return RedirectToAction("Product", "Product");
            //}
            //else
            //{
            //    // Redirect to the default Index action of Home controller
            //    return RedirectToAction("CustomerIndex", "Home");
            //}
        }


        // Action for storing the user's data after registration or sign up
        //public IActionResult Register(User user)
        //{
        //    // Validate required fields
        //    if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.UserName))
        //    {
        //        ModelState.AddModelError("", "Username, Email, and Password are required.");
        //        return View("Authentication", user); // Return with validation error
        //    }

        //    // Convert email to lowercase for consistent checks
        //    user.Email = user.Email.ToLower();

        //    // Check if email already exists (case-insensitive)
        //    if (_context.Users.Any(u => u.Email.ToLower() == user.Email))
        //    {
        //        ModelState.AddModelError("", "Email is already in use.");
        //        return View("Authentication", user);
        //    }

        //    // Assign default role based on email
        //    user.UserRole = user.Email.Contains("@admin") ? "Admin" : "Customer";

        //    // Securely hash the password before storing it
        //    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        //    // Save user to database
        //    _context.Users.Add(user);
        //    _context.SaveChanges();

        //    // Store user information in session
        //    HttpContext.Session.SetInt32("UserId", user.Id);
        //    HttpContext.Session.SetString("UserName", user.UserName);
        //    HttpContext.Session.SetString("Email", user.Email);
        //    HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
        //    HttpContext.Session.SetString("Address", user.Address ?? string.Empty);
        //    HttpContext.Session.SetString("UserRole", user.UserRole);

        //    // Redirect based on role
        //    return user.UserRole == "Admin"
        //        ? RedirectToAction("Product", "Product")   // Admin dashboard
        //        : RedirectToAction("CustomerIndex", "Home"); // Customer homepage
        //}


        // Action for logging in the user
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Login(string email, string password)
        //{
        //    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        //    {
        //        ViewBag.ErrorMessage = "Email and Password are required.";
        //        return View("Authentication");
        //    }

        //    var user = _context.Users.FirstOrDefault(u => u.Email == email);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))  // Secure password check
        //    {
        //        ViewBag.ErrorMessage = "Invalid email or password.";
        //        return View("Authentication");
        //    }

        //    // Store user information in the session
        //    HttpContext.Session.SetInt32("UserId", user.Id);
        //    HttpContext.Session.SetString("UserName", user.UserName);
        //    HttpContext.Session.SetString("Email", user.Email);
        //    HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
        //    HttpContext.Session.SetString("Address", user.Address ?? string.Empty);

        //    // Use a role-based check instead of checking for "@admin" in email
        //    if (user.UserRole == "Admin")
        //    {
        //        return RedirectToAction("Product", "Product");
        //    }

        //    return RedirectToAction("CustomerIndex", "Home");
        //}

        // Action for logging in the user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            // Validate required fields
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Email and Password are required.";
                ModelState.AddModelError("", "Username, Email, and Password are required.");
                return View("Authentication", user); // Return with validation error
            }

            if (user == null)
            {
                //_logger.LogError($"User not found: {email}");
                //ViewBag.ErrorMessage = "Invalid email or password";
                //return View("LoginPage");
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Invalid email or password";
                ModelState.AddModelError("", "Enter valid credentials.");
                return View("Authentication", user); // Return with validation error
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Inactive user login attempt: {email}");
                TempData["ShowPopup"] = true; // Indicate that the popup should be shown
                TempData["PopupMessage"] = "Your account is inactive. Please contact support: 09123456789 or gizmode@gmail.com";
                ModelState.AddModelError("", "Inactive account.");
                return View("Authentication", user); // Return with validation error
               
                //ViewBag.ErrorMessage = "Your account is inactive. Please contact support.";
                //return View("Authentication");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))  // Secure password check
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("Authentication");
            }

            if (user != null)
            {
                // Store user information in the session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? string.Empty);
                HttpContext.Session.SetString("Address", user.Address ?? string.Empty);

                // Update LastLogin timestamp
                user.LastLogin = DateTime.UtcNow;
                _context.SaveChanges();

                // Check if the email contains "@admin"
                // if (email.Contains("@admin"))
                if (user.UserRole == "Admin")
                {
                    // Redirect to the desired controller and action for admin
                    return RedirectToAction("Product", "Product");
                }
                else
                {
                    // Redirect to the default Index action of Home controller
                    return RedirectToAction("CustomerIndex", "Home");
                }

                //    // Use a role-based check instead of checking for "@admin" in email
                //    if (user.UserRole == "Admin")
                //    {
                //        return RedirectToAction("Product", "Product");
                //    }

                //    return RedirectToAction("CustomerIndex", "Home");
                //}
            }

           // ViewBag.ErrorMessage = "Invalid email or password";
            TempData["ShowPopup"] = true; // Indicate that the popup should be shown
            TempData["PopupMessage"] = "Invalid email or password";
            ModelState.AddModelError("", "Invalid input.");
            return View("Authentication");
        }

        // Background Service for checking inactive users and setting their last login date

        public class InactiveUserChecker : BackgroundService
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public InactiveUserChecker(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var threshold = DateTime.UtcNow.AddMonths(-6);
                    var inactiveUsers = dbContext.Users.Where(u => u.LastLogin < threshold && u.IsActive).ToList();

                    foreach (var user in inactiveUsers)
                    {
                        user.IsActive = false;
                    }

                    await dbContext.SaveChangesAsync();
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Run daily
                }
            }
        }
        //    public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();
        //    //HttpContext.Session.Remove("UserId");
        //    //HttpContext.Session.Remove("UserFullName");
        //    //HttpContext.Session.Remove("UserEmail");
        //    // Prevent back navigation from showing cached content
        //    Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //    Response.Headers["Pragma"] = "no-cache";
        //    Response.Headers["Expires"] = "0";

        //    return RedirectToAction("Authentication");
        //}

        // //if using Authentication
        public async Task<IActionResult> Logout()
        {
            // Sign out user
            await HttpContext.SignOutAsync();

            // Clear session
            HttpContext.Session.Clear();

            // Prevent back navigation from showing cached content
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Authentication");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}

