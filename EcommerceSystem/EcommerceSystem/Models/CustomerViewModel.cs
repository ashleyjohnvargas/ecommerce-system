using Microsoft.AspNetCore.Mvc;

namespace EcommerceSystem.Models
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        // public string? Status { get; set; } // Use this field for Active/Inactive
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } // For dropdown value



    }
}
