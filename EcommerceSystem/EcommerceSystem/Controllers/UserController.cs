using EcommerceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace EcommerceSystem.Controllers
{
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Customers()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve the user ID from the session

            if (userId == null)
            {
                //TempData["ErrorMessage"] = "You need to be logged in to access this page.";
                return RedirectToAction("Authentication", "Account");  // Adjust according to your login page
            }
            //// Step 1: Retrieve the user ID from the session
            //int? userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Authentication", "Account"); // Redirect to login if user is not logged in
            //}
            try
            {
                // Fetch data from the users table
                var activeUsers = await _context.Users
                    .Where(u => u.UserRole == "Customer")

                    //.Where(u => u.IsActive == true) // Filter only active users
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        IsActive = u.IsActive,

                    })
                    .ToListAsync();

                // Pass the list of users to the view
                return View(activeUsers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
                return View(new List<UserViewModel>()); // Return an empty list in case of error
            }
        }


        //public async Task<IActionResult> Users()
        //{
        //    try
        //    {
        //        // Fetch data from the users table
        //        var activeUsers = await _context.Users
        //            //.Where(u => u.IsActive == true) // Filter only active users
        //            .Select(u => new UserViewModel
        //            {
        //                Id = u.Id,
        //                FullName = u.UserName,
        //                Email = u.Email,
        //                IsActive = u.IsActive,

        //            })
        //            .ToListAsync();

        //        // Pass the list of users to the view
        //        return View(activeUsers);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
        //        return View(new List<UserViewModel>()); // Return an empty list in case of error
        //    }
        //}
        // GET: Display the Add User form
        [HttpGet]
        public IActionResult AddUser()
        {
            // Step 1: Retrieve the user ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // Redirect to login if user is not logged in
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // or any page you prefer
            }

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("AddUser");
            }

            // Hash the password before saving
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newUser = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                // Status = Status,  // "Active" or "Inactive"
                Password = hashedPassword,  // Store the hashed password
                IsActive = model.IsActive = true,// Set the IsActive based on the status
                UserRole = "Admin"  // Set user role to "Admin"
            };

            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();


                //var newUserProfile = new Profile
                //{
                //    FullName = newUser.UserName,
                //    Email = newUser.Email,
                //    Id = newUser.Id  // Link the profile to the newly created user
                //};

                //_context.UserProfiles.Add(newUserProfile);
                //await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction("Users"); // Redirect to the users list or another page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the user: " + ex.Message;
                return RedirectToAction("AddUser");
            }

        }

        // Display the Edit User page
        public IActionResult EditUser(int id)
        {
            // Step 1: Retrieve the user ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // Redirect to login if user is not logged in
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Users");
            }

            // Pass the user data to the view
            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return View(model);
        }

       
        // Handle form submission to update user
        [HttpPost]
        public IActionResult EditUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Users");
            }

            // Disable the foreign key constraint temporarily
           // _context.Database.ExecuteSqlRaw("ALTER TABLE UserProfiles NOCHECK CONSTRAINT FK_UserProfiles_Users_Email");

            try
            {
                // Update the email in UserProfiles first
                //var userProfile = _context.UserProfiles.FirstOrDefault(up => up.Email == user.Email);
                //if (userProfile != null)
                //{
                //    // Update the email in UserProfiles
                //    userProfile.Email = model.Email;
                //}

                // Update user details
                //user.UserName = model.UserName;
                user.Email = model.Email; // Update email in Users table after UserProfiles
                user.IsActive = model.Status == "Active"; // Map dropdown value to boolean

                // Update password if provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "User updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error occurred: {ex.Message}";
            }
            //finally
            //{
            //    // Re-enable the foreign key constraint and validate data integrity
            //    _context.Database.ExecuteSqlRaw("ALTER TABLE UserProfiles WITH CHECK CHECK CONSTRAINT FK_UserProfiles_Users_Email");
            //}

            return RedirectToAction("Users");
        }

       
        //[HttpPost]
        //public IActionResult EditUser(Profile model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Retrieve the user ID from session
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    // Retrieve the user from the database using the ID from session
        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "User not found.";
        //        return RedirectToAction("Authentication", "Account"); // If user is not found, redirect to login page
        //    }

        //    // Ensure the profile and user exist before updating
        //    if (user != null)
        //    {
        //        // Only update the fields if the user has provided new values
        //        if (!string.IsNullOrEmpty(model.UserName))
        //        {
        //            user.UserName = model.UserName;
        //        }

        //        if (!string.IsNullOrEmpty(model.Email))
        //        {
        //            user.Email = model.Email;
        //        }
        //        if (!string.IsNullOrEmpty(model.PhoneNumber))
        //        {
        //            user.PhoneNumber = model.PhoneNumber;
        //        }
        //        if (!string.IsNullOrEmpty(model.Address))
        //        {
        //            user.Address = model.Address;
        //        }
        //        if (!string.IsNullOrEmpty(model.Password))
        //        {
        //            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);  // Hash the new password before saving
        //        }

        //        // Save changes to the database
        //        _context.SaveChanges();

        //        // Set success message
        //        TempData["SuccessMessage"] = "Profile updated successfully!";
        //    }
        //    else
        //    {
        //        // Handle case where profile or user doesn't exist
        //        TempData["ErrorMessage"] = "Profile not found.";
        //        return RedirectToAction("CustomerIndex", "Home");
        //    }

        //    // Redirect to the profile page after update
        //    return RedirectToAction("CustomerIndex", "Home");
        //}

        // Handle form submission to update user
        [HttpPost]
        public IActionResult EditCustomer(UserViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Authentication", "Account"); // or any page you prefer
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Customers");
            }

            // Disable the foreign key constraint temporarily
            // _context.Database.ExecuteSqlRaw("ALTER TABLE UserProfiles NOCHECK CONSTRAINT FK_UserProfiles_Users_Email");

            try
            {
                // Update the email in UserProfiles first
                var userProfile = _context.UserProfiles.FirstOrDefault(up => up.Email == user.Email);
                if (userProfile != null)
                {
                    // Update the email in UserProfiles
                    userProfile.Email = model.Email;
                }

                // Update user details
                user.UserName = model.UserName;
                user.Email = model.Email; // Update email in Users table after UserProfiles
                user.IsActive = model.Status == "Active"; // Map dropdown value to boolean

                // Update password if provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "User updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error occurred: {ex.Message}";
            }
            //finally
            //{
            //    // Re-enable the foreign key constraint and validate data integrity
            //    _context.Database.ExecuteSqlRaw("ALTER TABLE UserProfiles WITH CHECK CHECK CONSTRAINT FK_UserProfiles_Users_Email");
            //}

            return RedirectToAction("Customers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Set IsActive to false (mark as inactive)
            user.IsActive = false;
            _context.Update(user);
            await _context.SaveChangesAsync();

            // Optionally, show a success message
            TempData["SuccessMessage"] = "User marked as inactive successfully.";

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Set IsActive to false (mark as inactive)
            user.IsActive = false;
            _context.Update(user);
            await _context.SaveChangesAsync();

            // Optionally, show a success message
            TempData["SuccessMessage"] = "User marked as inactive successfully.";

            return RedirectToAction("Customers");
        }


        //public async Task<IActionResult> Customers()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Users()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve the user ID from the session

            if (userId == null)
            {
                //TempData["ErrorMessage"] = "You need to be logged in to access this page.";
                return RedirectToAction("Authentication", "Account");  // Adjust according to your login page
            }

            //var userId = HttpContext.Session.GetInt32("UserId");

            //if (userId == null)
            //{
            //    return RedirectToAction("Authentication", "Account"); // or any page you prefer
            //}

            try
            {
                // Fetch data from the users table
                var users = await _context.Users
                    .Where(u => u.UserRole != "Customer")
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        //Role = u.UserRole,
                        IsActive = u.IsActive,

                    })
                    .ToListAsync();

                // Pass the list of users to the view
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
                return View(new List<UserViewModel>()); // Return an empty list in case of error
            }
        }

    }
}
