using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; } // Store hashed passwords

    }
}
