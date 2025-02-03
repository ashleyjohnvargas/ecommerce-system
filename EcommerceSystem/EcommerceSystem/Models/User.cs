using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class User
    {
        [Key]
        public int Id {get; set;}
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
        
        // New field for user roles (e.g., "Admin", "Customer")
        public string UserRole { get; set; } = "Customer"; // Default role
                                                           //public string? Status { get; set; } // Use this field for Active/Inactive
        public DateTime? LastLogin { get; set; }
        public DateTime? LastPasswordChange { get; set; }
    }
}
