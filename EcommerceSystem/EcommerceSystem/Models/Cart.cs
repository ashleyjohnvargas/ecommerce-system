using System.ComponentModel.DataAnnotations;

namespace EcommerceSystem.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; } // Primary Key
        public int? CustomerId { get; set; } // Foreign Key to Customer table
        public decimal? TotalPrice { get; set; } // Total price of items in the cart
        public string? Status { get; set; } // Inactive, Active, or Completed

        // Navigation property for multiple CartItems
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
