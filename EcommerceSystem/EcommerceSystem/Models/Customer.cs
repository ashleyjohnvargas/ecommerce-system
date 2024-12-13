using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class Customer
    {
        public int CustomerId { get; set; } // PK
        public required string Name { get; set; }    // Full name
        public required string Email { get; set; }  // Email
        public required string Password { get; set; } // Encrypted password
        public string? PhoneNumber { get; set; } // Nullable
        public string? Address { get; set; }     // Nullable
    }
}
