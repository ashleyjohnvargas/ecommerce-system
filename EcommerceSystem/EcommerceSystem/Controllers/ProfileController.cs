using EcommerceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace EcommerceSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //public ProfileController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        //public IActionResult Profile()
        //{
        //    return View();
        //}
        //public IActionResult CustomerProfile()
        //{
        //    return View();
        ////}
        //////GET: Customer/Profile
        //public IActionResult Profile(int id)
        //{
        //    // Replace with actual user ID or authentication logic
        //    var customer = _context.Users.FirstOrDefault(u => u.Id == id);

        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customer);
        //}

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve the user ID from the session

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to access this page.";
                return RedirectToAction("Authentication", "Account");  // Adjust according to your login page
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId); // Match with database

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Authentication", "Account");
            }

            var model = new Profile
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                //IsActive = user.IsActive
            };

            return View(model);
        }




        [HttpPost]
        public IActionResult EditProfile(Profile model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the user ID from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You need to be logged in to update your profile.";
                return RedirectToAction("Authentication", "Account");  // Redirect to login page if not logged in
            }

            // Retrieve the user from the database using the ID from session
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Authentication", "Account"); // If user is not found, redirect to login page
            }

            // Ensure the profile and user exist before updating
            if (user != null)
            {
                // Only update the fields if the user has provided new values
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    user.UserName = model.UserName;
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    user.PhoneNumber = model.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(model.Address))
                {
                    user.Address = model.Address;
                }
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);  // Hash the new password before saving
                }

                // Save changes to the database
                _context.SaveChanges();

                // Set success message
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            else
            {
                // Handle case where profile or user doesn't exist
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("CustomerIndex", "Home");
            }

            // Redirect to the profile page after update
            return RedirectToAction("EditProfile");
        }
        //[HttpPost]
        //public IActionResult EditProfile(UserViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Retrieve the user ID from session
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "You need to be logged in to update your profile.";
        //        return RedirectToAction("Authentication", "Account");  // Redirect to login page if not logged in
        //    }

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

        //       // Save changes to the database
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
        //try
        //{

        //    // Update user details with the data from the form (model)
        //    user.UserName = model.UserName;
        //    user.Email = model.Email;
        //    user.PhoneNumber = model.PhoneNumber;
        //    user.Address = model.Address;
        //    //user.IsActive = model.Status == "Active"; // Assuming 'Status' is used for active status

        //    // Update password if provided
        //    if (!string.IsNullOrEmpty(model.Password))
        //    {
        //        user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);  // Hash the new password before saving
        //    }

        //    // Save changes to the database
        //    _context.SaveChanges();

        //    TempData["SuccessMessage"] = "Profile updated successfully!";
        //    return RedirectToAction("CustomerIndex", "Home");  // Stay on the Edit Profile page after successful update
        //}
        //catch (Exception ex)
        //{
        //    TempData["ErrorMessage"] = $"Error occurred: {ex.Message}";
        //    return View(model);  // Return to the form if an error occurs
        //}
        //}


        // Display the Edit Profile page for the signed-in user
        //public IActionResult EditProfile()
        //{
        //    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; // Get logged-in user ID
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        TempData["ErrorMessage"] = "User not found.";
        //        return RedirectToAction("Authentication", "Account"); // Redirect to a relevant page
        //    }

        //    var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId); // Match with database

        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "User not found.";
        //        return RedirectToAction("Authentication", "Account");
        //    }

        //    var model = new UserViewModel
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        Address = user.Address,
        //        IsActive = user.IsActive
        //    };

        //    return View(model);
        //}


        //// Handle form submission to update profile
        //[HttpPost]
        //public IActionResult EditProfile(UserViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var userEmail = User.Identity.Name;
        //    var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

        //    if (user == null || user.Id != model.Id) // Prevent editing other users
        //    {
        //        TempData["ErrorMessage"] = "Unauthorized action.";
        //        return RedirectToAction("Authentication", "Account");
        //    }

        //    try
        //    {
        //        // Update email in UserProfiles if necessary
        //        var userProfile = _context.UserProfiles.FirstOrDefault(up => up.Email == user.Email);
        //        if (userProfile != null)
        //        {
        //            userProfile.Email = model.Email;
        //            userProfile.PhoneNumber = model.PhoneNumber;
        //            userProfile.Address = model.Address;
        //        }

        //        // Update user details
        //        user.UserName = model.UserName;
        //        user.Email = model.Email;
        //        user.PhoneNumber = model.PhoneNumber;
        //        user.Address = model.Address;
        //        user.IsActive = model.Status == "Active";

        //        // Update password if provided
        //        if (!string.IsNullOrEmpty(model.Password))
        //        {
        //            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        //        }

        //        _context.SaveChanges();
        //        TempData["SuccessMessage"] = "Profile updated successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error occurred: {ex.Message}";
        //    }

        //    return RedirectToAction("EditProfile"); // Stay on the profile page
        //}

        // Display the Edit User page
        //public IActionResult EditProfile(int id)
        //{
        //    var user = _context.Users.FirstOrDefault(u => u.Id == id);
        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "User not found.";
        //        return RedirectToAction("Users");
        //    }

        //    // Pass the user data to the view
        //    var model = new UserViewModel
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        IsActive = user.IsActive
        //    };

        //    return View(model);
        //}

        // Handle form submission to update user
        //[HttpPost]
        //public IActionResult EditProfile(UserViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "User not found.";
        //        return RedirectToAction("Profile");
        //    }

        //    // Disable the foreign key constraint temporarily
        //    // _context.Database.ExecuteSqlRaw("ALTER TABLE UserProfiles NOCHECK CONSTRAINT FK_UserProfiles_Users_Email");

        //    try
        //    {
        //        // Update the email in UserProfiles first
        //        var userProfile = _context.UserProfiles.FirstOrDefault(up => up.Email == user.Email);
        //        if (userProfile != null)
        //        {
        //            // Update the email in UserProfiles
        //            userProfile.Email = model.Email;
        //        }

        //        // Update user details
        //        user.UserName = model.UserName;
        //        user.Email = model.Email; // Update email in Users table after UserProfiles
        //        user.IsActive = model.Status == "Active"; // Map dropdown value to boolean

        //        // Update password if provided
        //        if (!string.IsNullOrEmpty(model.Password))
        //        {
        //            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
        //        }

        //        _context.SaveChanges();

        //        TempData["SuccessMessage"] = "User updated successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error occurred: {ex.Message}";
        //    }
        //    //finally
        //    //{
        //    //    // Re-enable the foreign key constraint and validate data integrity
        //    //    _context.Database.ExecuteSqlRaw("ALTER TABLE UserProfiles WITH CHECK CHECK CONSTRAINT FK_UserProfiles_Users_Email");
        //    //}

        //    return RedirectToAction("Profile");
        //}

        //// POST: Customer/UpdateProfile
        //[HttpPost]
        //public IActionResult UpdateProfile(Profile model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var customer = _context.Users.FirstOrDefault(u => u.Id == model.Id);
        //        if (customer != null)
        //        {
        //            customer.UserName = model.FullName;
        //            customer.Email = model.Email;
        //            customer.PhoneNumber = model.PhoneNumber;
        //            customer.Address = model.Address;
        //            //customer.ProfilePicture = model.ProfilePicture; // Handle picture upload logic here

        //            _context.SaveChanges();
        //            return RedirectToAction("Profile", new { id = model.Id });
        //        }
        //    }

        //    return View(model);
        //}

        // GET: /Customer/Profile
        //public IActionResult Profile(int customerId)
        //{
        //    var customer = _context.CustomerProfiles.Find(customerId);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: /Customer/Profile
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Profile(CustomerProfile profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(profile);
        //        _context.SaveChanges();
        //        return RedirectToAction("Profile", new { customerId = profile.Id });
        //    }
        //    return View(profile);
        //}
    }
}

