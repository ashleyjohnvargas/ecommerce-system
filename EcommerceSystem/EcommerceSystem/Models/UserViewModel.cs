using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using EcommerceSystem.Controllers;
using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class UserViewModel
    {
        public int Id { get; set; } // Primary Key
        public string? UserName { get; set; } // User's full name
        public string? Email { get; set; } // User's email
        public bool IsActive { get; set; } = true;
        // public string? Status { get; set; } // Use this field for Active/Inactive
        public string Status { get; set; } // For dropdown value

       
                                              //public string Status => IsActive ? "Active" : "Inactive";
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public string? Password { get; set; } // Store hashed passwords


    }
}
